using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public Controls controls;
    private LayerMask ground = 1 << 6;

    public PlayerType playerClass;
    public AbilityType abilityClass;
    public WeaponType weaponClass;

    private AlterableStats hpMana;

    public ItemType currentItem;

    private float currentSpeed;

    private Rigidbody2D rb2D;
    private Animator anim;
    private LineRenderer lineRndr;

    private GameObject weapon;
    [HideInInspector] public GameObject weaponPivot;

    [HideInInspector] public int playerNum;

    // HUD Settings.
    private UnityEngine.UI.Image playerIcon;
    private UnityEngine.UI.Image weaponIcon;
    private UnityEngine.UI.Image abilityIcon;

    private UnityEngine.UI.Image healthBar;
    private UnityEngine.UI.Image manaBar;

    public GameObject arrow;

    private int arrowCount;
    private float bowForce;
    private float bowPullTime;
    private float maxBowPullTime;

    public GameObject spellBolt;

    private bool isWeaponUpdate;
    private bool isAbilityUpdate;

    [Header("Boxcast Settings")]
    public Vector2 boxSize;
    public Vector2 boxOffset;

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
        controls.Player.UseItem.performed += _ => UseItem();

        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lineRndr = GetComponent<LineRenderer>();

        weapon = GetComponentInChildren<Weapon>().gameObject;
        weaponPivot = weapon.transform.parent.gameObject;

        playerClass = PlayerTypes.PorkChops;
        weaponClass = WeaponTypes.Bow;
        abilityClass = AbilityTypes.DirtDash;

        currentSpeed = playerClass.walkSpeed;
        hpMana = new AlterableStats(playerClass.hp, playerClass.mana);

        #region HUD Setting

        UnityEngine.UI.Image[] hud = GameObject.FindGameObjectsWithTag("HUD")[playerNum].GetComponentsInChildren<UnityEngine.UI.Image>();

        healthBar = hud[0];
        manaBar = hud[1];

        playerIcon = hud[2];
        playerIcon.sprite = playerClass.icon;

        weaponIcon = hud[3];
        weaponIcon.sprite = weaponClass.icon;

        abilityIcon = hud[4];
        abilityIcon.sprite = abilityClass.icon;

        if (playerClass.mana == 0)
        {
            manaBar.enabled = false;
        }

        #endregion
    }

    private void Update()
    {
        Move(controls.Player.Movement.ReadValue<Vector2>());

        if (isWeaponUpdate)
        {
            WeaponUpdate();
        }

        if (isAbilityUpdate)
        {
            AbilityUpdate();
        }
    }

    #region Inputs

    private void Move(Vector2 input)
    {
        rb2D.velocity = new Vector2(input.x * currentSpeed, rb2D.velocity.y);
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
        if (weaponClass.manaUse > hpMana.currentMana)
            return;

        UseMana(weaponClass.manaUse);
        weaponClass.actionStart.Invoke(this);

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
        if (abilityClass.manaUse > hpMana.currentMana)
            return;

        UseMana(abilityClass.manaUse);
        abilityClass.actionStart.Invoke(this);

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

    private void UseItem()
    {
        if (currentItem == null)
            return;

        currentItem.useAction.Invoke();
    }

    #endregion

    public void Mine(float size)
    {

    }

    #region Weapons

    public void Drill()
    {

    }

    public void Pickaxe()
    {

    }

    public void Spell()
    {

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
            GameObject newArrow = Instantiate(arrow);
            newArrow.transform.position = transform.position;

            newArrow.GetComponent<Rigidbody2D>().AddForce(weaponPivot.transform.right * bowPullTime * 10f * bowForce, ForceMode2D.Impulse);
            newArrow.GetComponent<Arrow>().damage = weaponClass.damage;
        }

        bowPullTime = 0f;
        RenderThrowingArc(Vector2.zero, 0, 0);
    }

    #endregion

    #region Abilities

    public void Dash()
    {

    }

    #endregion

    #region Stats

    public float CalcDamage()
    {
        return playerClass.damage * weaponClass.damage;
    }

    public void DealDamage(PlayerController target, float damage)
    {
        target.TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        hpMana.currentHP -= (damage > hpMana.currentHP ? hpMana.currentHP : damage);

        healthBar.fillAmount = hpMana.currentHP / playerClass.hp;
    }

    public void UseMana(float manaUse)
    {
        hpMana.currentMana -= manaUse;

        manaBar.fillAmount = hpMana.currentMana/ playerClass.mana;
    }

    #endregion

    #region Misc

    private bool IsGrounded()
    {
        return Physics2D.BoxCast((Vector2) transform.position + boxOffset, boxSize, 0, Vector2.down, 0f, ground);
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
