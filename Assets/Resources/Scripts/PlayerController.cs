using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
public class PlayerController : MonoBehaviour, IComparable
{
    private Controls controls;

    private LayerMask ground = 1 << 6;
    private LayerMask item = 1 << 9;

    public PigType playerClass;
    public AbilityType abilityClass;
    public WeaponType weaponClass;

    public AlterableStats alterableStats;
    public float statMultiplier = 1f;

    [HideInInspector] public Item currentItem;

    private float currentSpeed;

    private Rigidbody2D rb2D;
    private Animator anim;
    private LineRenderer lineRndr;
    private BoxCollider2D col2D;
    private SpriteRenderer sprtRndr;

    [HideInInspector] public Weapon weapon;
    [HideInInspector] public GameObject weaponPivot;

    [HideInInspector] public int playerNum;
    [HideInInspector] public int teamNum;

    // HUD Settings.
    private UnityEngine.UI.Image playerIcon;
    private UnityEngine.UI.Image weaponIcon;
    private UnityEngine.UI.Image abilityIcon;

    private UnityEngine.UI.Image healthBar;
    private UnityEngine.UI.Image manaBar;
    private TMPro.TMP_Text stockCount;

    [SerializeField] private GameObject arrow;

    private int arrowCount;
    private float bowForce;
    private float bowPullTime;
    private float maxBowPullTime;

    [SerializeField] private GameObject spellBolt;

    private bool isWeaponUpdate;
    private bool isAbilityUpdate;

    private bool canDash;
    private Vector2 dashVec;
    private bool isDashDigging;

    [Header("Boxcast Settings")]
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private Vector2 boxOffset;

    [HideInInspector] public float relicTime;

    private void Awake()
    {
        controls = new Controls();

        // Movement.
        controls.Player.Aiming.performed += ctx => Aim(ctx.ReadValue<Vector2>().normalized);
        controls.Player.Run.performed += ctx => Run(ctx.ReadValue<float>() == 1f);
        controls.Player.Jump.performed += _ => Jump();

        // Combat.
        controls.Player.UseWeapon.started += _ => WeaponStart();
        controls.Player.UseWeapon.canceled += _ => WeaponEnd();
        controls.Player.UseAbility.started += _ => AbilityStart();
        controls.Player.UseAbility.canceled += _ => AbilityEnd();

        // Misc.
        controls.Player.UseItem.started += _ => PickupOrUseItem();

        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lineRndr = GetComponent<LineRenderer>();
        col2D = GetComponent<BoxCollider2D>();
        sprtRndr = GetComponent<SpriteRenderer>();

        weapon = GetComponentInChildren<Weapon>();
        weaponPivot = weapon.transform.parent.gameObject;
    }

    private void Update()
    {
        if (!isDashDigging)
        {
            if (dashVec.magnitude > 0.25f)
            {
                dashVec -= dashVec.normalized * Time.deltaTime * 100f;
                rb2D.velocity = dashVec;
            }
            else
            {
                Move(controls.Player.Movement.ReadValue<Vector2>());
                dashVec = Vector2.zero;
            }
        }

        if (currentItem && currentItem.item == ItemTypes.Relic)
        {
            relicTime += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (isWeaponUpdate)
        {
            WeaponUpdate();
        }
        if (isAbilityUpdate)
        {
            AbilityUpdate();
        }
    }

    public void Mine()
    {
        GameController.TerrainInterface.DestroyTerrain(transform.position + weaponPivot.transform.right, weaponClass.mineRadius, out bool hasMined);

        if (hasMined)
        {
            rb2D.velocity = weaponPivot.transform.right * playerClass.undergroundSpeed;
        }
    }

    #region Setup

    public void Setup(Player player, PigType pigType, WeaponType weaponType, AbilityType abilityType, int playerNum, int teamNum)
    {
        playerClass = pigType;
        weaponClass = weaponType;
        abilityClass = abilityType;

        this.playerNum = playerNum;
        this.teamNum = teamNum;

        currentSpeed = playerClass.walkSpeed;
        alterableStats = new AlterableStats(playerClass.hp, playerClass.mana, GameController.gameSettings.stocks, 0, 0, 0f);

        player.user.AssociateActionsWithUser(controls);

        InputControlScheme? scheme = InputControlScheme.FindControlSchemeForDevice(player.pairedDevice, controls.controlSchemes);
        if (scheme.HasValue)
        {
            player.user.ActivateControlScheme(scheme.Value);
        }

        Spawn();
        SetupHUD();
    }

    private void Spawn()
    {
        transform.position = GameController.spawnPoints[playerNum].transform.position;

        alterableStats.currentHP = playerClass.hp;
        alterableStats.currentMana = playerClass.mana;
    }

    private void SetupHUD()
    {
        GameObject hud = GameObject.FindGameObjectsWithTag("HUD")[playerNum];
        UnityEngine.UI.Image[] hudImages = hud.GetComponentsInChildren<UnityEngine.UI.Image>();

        healthBar = hudImages[0];
        manaBar = hudImages[1];

        playerIcon = hudImages[2];
        playerIcon.sprite = playerClass.icon;

        weaponIcon = hudImages[3];
        weaponIcon.sprite = weaponClass.icon;

        abilityIcon = hudImages[4];
        abilityIcon.sprite = abilityClass.icon;

        stockCount = hud.GetComponentsInChildren<TMPro.TMP_Text>()[1];

        if (playerClass.mana == 0)
        {
            manaBar.enabled = false;
        }

        stockCount.text = "x" + alterableStats.stocks;
    }

    #endregion

    #region Inputs

    private void Move(Vector2 input)
    {
        rb2D.velocity = new Vector2(input.x * currentSpeed * statMultiplier, rb2D.velocity.y);

        if (rb2D.velocity.x < 0f)
        {
            sprtRndr.flipX = true;
        }
        else if (rb2D.velocity.x > 0f)
        {
            sprtRndr.flipX = false;
        }

        if (controls.Player.Aiming.ReadValue<Vector2>() == Vector2.zero)
        {
            Aim(input);
        }
    }

    private void Aim(Vector2 input)
    {
        weaponPivot.transform.right = input;
    }

    private void Run(bool input)
    {
        this.currentSpeed = input ? playerClass.runSpeed : playerClass.walkSpeed;
    }

    private void Jump()
    {
        if (!IsGrounded())
            return;

        rb2D.AddForce(new Vector2(0f, playerClass.jumpHeight), ForceMode2D.Impulse);
    }

    // Called when the player begins to use a weapon.
    private void WeaponStart()
    {
        if (weaponClass.manaUse > alterableStats.currentMana)
            return;

        if (weaponClass.actionStart != null)
        {
            weaponClass.actionStart.Invoke(this);
        }
        UseMana(weaponClass.manaUse);

        if (weaponClass.actionUpdate != null)
        {
            isWeaponUpdate = true;
        }
    }

    // Called until the player finishes using a weapon.
    private void WeaponUpdate()
    {
        weaponClass.actionUpdate.Invoke(this);
    }

    // Called when the player finishes using a weapon.
    private void WeaponEnd()
    {
        if (weaponClass.actionUpdate != null)
        {
            isWeaponUpdate = false;
        }

        if (weaponClass.actionEnd == null)
            return;

        weaponClass.actionEnd.Invoke(this);
    }

    // Called when the player begins to use an ability.
    private void AbilityStart()
    {
        if (abilityClass.manaUse > alterableStats.currentMana)
            return;

        if (abilityClass.actionStart != null)
        {
            abilityClass.actionStart.Invoke(this);
        }
        UseMana(abilityClass.manaUse);

        if (abilityClass.actionUpdate != null)
        {
            isAbilityUpdate = true;
        }
    }

    // Called until the player finishes using an ability.
    private void AbilityUpdate()
    {
        abilityClass.actionUpdate.Invoke(this);
    }

    // Called when the player finishes using an ability.
    private void AbilityEnd()
    {
        if (abilityClass.actionUpdate != null)
        {
            isAbilityUpdate = false;
        }

        if (abilityClass.actionEnd == null)
            return;

        abilityClass.actionEnd.Invoke(this);
    }

    private void PickupOrUseItem()
    {
        if (currentItem != null)
        {
            UseItem();
        }
        else
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        RaycastHit2D results = Physics2D.BoxCast(transform.position, transform.localScale, 0f, Vector2.zero, 0f, item);

        if (results && results.transform.gameObject != null)
        {
            Item item = results.transform.gameObject.GetComponent<Item>();

            currentItem = item;
            item.carrier = this;

            if (item.item.pickupAction != null)
            {
                item.item.pickupAction.Invoke(item);
            }
        }
    }

    private void UseItem()
    {
        if (currentItem.item.useAction != null)
        {
            currentItem.item.useAction.Invoke(currentItem);
        }
        currentItem = null;
    }

    #endregion
    
    #region Weapons

    public void Spell()
    {
        GameObject newSpell = Instantiate(spellBolt);
        newSpell.transform.position = transform.position;

        newSpell.GetComponent<Rigidbody2D>().velocity = (weaponPivot.transform.right * 10f);
        newSpell.GetComponent<Projectile>().owner = this;
    }

    public void BowStart(int arrowCount, float distance, float maxPullTime)
    {
        this.arrowCount = arrowCount;
        bowForce = distance;
        maxBowPullTime = maxPullTime;

        bowPullTime = 0.01f;
    }

    public void BowUpdate()
    {
        if ((bowPullTime += Time.deltaTime) > maxBowPullTime)
        {
            bowPullTime = maxBowPullTime;
        }

        RenderThrowingArc(weaponPivot.transform.right, bowPullTime * 3.125f * bowForce, 10);
    }

    public void BowEnd()
    {
        for (int i  = 0; i < arrowCount; ++i)
        {
            Projectile newArrow = Instantiate(arrow).GetComponent<Projectile>();
            newArrow.owner = this;
            newArrow.transform.position = transform.position;

            newArrow.Shoot(weaponPivot.transform.right * bowPullTime * 10f * bowForce);
        }

        bowPullTime = 0f;
        RenderThrowingArc(Vector2.zero, 0, 0);
    }

    #endregion

    #region Abilities

    public void Dash(float dashDist)
    {
        if (!canDash)
            return;

        rb2D.velocity = Vector2.zero;
        dashVec = controls.Player.Movement.ReadValue<Vector2>().normalized * dashDist;
        canDash = false;
    }

    #endregion

    #region Stats

    public static float CalcDamage(PlayerController attacker, PlayerController defender)
    {
        return (attacker.weaponClass.damage * attacker.playerClass.damage * attacker.statMultiplier) / (defender.playerClass.defense * defender.statMultiplier);
    }

    public void DealDamage(PlayerController target)
    {
        target.TakeDamage(CalcDamage(this, target), this);
        target.TakeKnockback(transform.position, this.weaponClass.knockback);
    }

    public void TakeDamage(float damage, PlayerController attacker)
    {
        alterableStats.currentHP -= (damage > alterableStats.currentHP ? alterableStats.currentHP : damage);
        UpdateHealth();

        if (alterableStats.currentHP == 0f)
        {
            ++attacker.alterableStats.kills;
            Die();
        }
    }

    public void TakeKnockback(Vector2 hitLoc, float force)
    {
        rb2D.AddForce(((hitLoc - (Vector2) transform.position).normalized) * force);
    }

    public void UseMana(float manaUse)
    {
        alterableStats.currentMana -= manaUse;
        UpdateMana();
    }

    public void Heal(float amount)
    {
        alterableStats.currentHP += amount < playerClass.hp - alterableStats.currentHP ? amount : playerClass.hp - alterableStats.currentHP;
        UpdateHealth();
    }

    public void RestoreMana(float amount)
    {
        alterableStats.currentMana += amount < playerClass.mana - alterableStats.currentMana ? amount : playerClass.mana - alterableStats.currentMana;
        UpdateMana();
    }

    public void Die()
    {
        --alterableStats.stocks;
        stockCount.text = "x" + alterableStats.stocks;

        if (alterableStats.stocks > 0)
        {
            LosePoints(5);

            Spawn();

            UpdateHealth();
            UpdateMana();
        }
        else
        {
            GameController.current.playersLeft.Remove(this);
            gameObject.SetActive(false);
        }
    }

    private void LosePoints(int count)
    {
        this.alterableStats.points -= count > this.alterableStats.points ? this.alterableStats.points : count;
    }

    private void UpdateHealth()
    {
        healthBar.fillAmount = alterableStats.currentHP / playerClass.hp;
    }

    private void UpdateMana()
    {
        manaBar.fillAmount = alterableStats.currentMana / playerClass.mana;
    }

    #endregion

    #region Misc

    private bool IsGrounded()
    {
        bool isGrounded = Physics2D.BoxCast((Vector2)transform.position + boxOffset, boxSize, 0, Vector2.down, 0f, ground);

        if (isGrounded)
        {
            canDash = true;
        }

        return isGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube((Vector2) transform.position + boxOffset, boxSize);
    }

    private void RenderThrowingArc(Vector2 throwVec, float velocity, int resolution)
    {
        lineRndr.positionCount = resolution;

        float angle = Mathf.Atan2(throwVec.y, throwVec.x);
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * angle)) / 2f;

        lineRndr.SetPositions(CalculateThrowingPositions(resolution, maxDistance, angle, velocity));
    }

    private Vector3[] CalculateThrowingPositions(int resolution, float maxDistance, float angle, float velocity)
    {
        Vector3[] positions = new Vector3[resolution];

        for (int i = 0; i < resolution; ++i)
        {
            float t = (float)i / (float)resolution;
            positions[i] = CalculateArcPoint(t, maxDistance, angle, velocity);
        }

        return positions;
    }

    private Vector2 CalculateArcPoint(float t, float maxDistance, float angle, float velocity)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(angle) - (2f * x * x / (2 * velocity * velocity * Mathf.Cos(angle) * Mathf.Cos(angle)));

        return new Vector2(x, y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dashVec.magnitude > 0.125f && collision.gameObject.layer << ground != 0)
        {
            isDashDigging = true;

            col2D.isTrigger = true;
            rb2D.gravityScale = 0f;
            rb2D.velocity = playerClass.undergroundSpeed * dashVec.normalized * 1.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isDashDigging)
            return;

        if (collision.gameObject.layer << ground != 0)
        {
            isDashDigging = false;
            canDash = true;

            col2D.isTrigger = false;
            rb2D.gravityScale = 5f;
            rb2D.velocity = Vector2.zero;
        }
    }

    int IComparable.CompareTo(object obj)
    {
        PlayerController player = (PlayerController)obj;
        return this.playerNum.CompareTo(player.playerNum);
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    #endregion
}
