using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : MonoBehaviour
{
    public static readonly int PlayerLayer = 8;
    public static readonly int EnemyLayer = 9;
    public bool IsPlayer;
    //public string data;
    //public static int popCost;
    //public static int foodCost;
    public UnitType Type;
    public RawImage HealthBar;
    //Atributes
    public float Speed = 1.0f;
    public static float AttackSpeed = 1.0f;
    public float AttackDistance = 0.0005f;
    public float StopDistance = 0.5f;
    public int InitialHealth = 100;
    public float RaycastOffset = 0.1f;
    private bool atacando = false;
    //Privates
    private Player Player;
    private Vector3 MovementDir;
    private Animator Animator;
    //IDs
    private int IsWalkingID;
    private int AttackID;
    private int DieID;
    private int EnemyTeamLayer;
    //
    private GameObject Enemy;
    protected int Health;
    public bool Dead { get; private set; }
    //private SpriteRenderer Image;
    //private Color ImageColor;

    public enum UnitType{
        Soldier, Knight, Barbarian, Catapult
    }

    public virtual void Awake()
    {
        Player = GetComponentInParent<Player>();
    }
    void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        IsWalkingID = Animator.StringToHash("isWalking");
        AttackID = Animator.StringToHash("attack");
        DieID = Animator.StringToHash("die");
        MovementDir = IsPlayer ? Vector3.forward : Vector3.back;
        EnemyTeamLayer = IsPlayer ? EnemyLayer : PlayerLayer;
        Animator.SetFloat("attackSpeed", AttackSpeed);

         Health = InitialHealth;
         foreach (Transform t in GetComponentsInChildren<Transform>()) t.gameObject.layer = IsPlayer? PlayerLayer:EnemyLayer;
         if (!IsPlayer)
        {
            Quaternion localRotation = transform.localRotation;
            localRotation.y = 180;
            transform.localRotation = localRotation;
        }
    }

    
    void Update()
    {
        if (Health == 0) return;
        Debug.DrawRay(transform.position + MovementDir * RaycastOffset, MovementDir.normalized * AttackDistance, Color.red);
        RaycastHit[] hits = Physics.RaycastAll(transform.position + MovementDir * RaycastOffset, MovementDir, AttackDistance, 1<<PlayerLayer | 1<<EnemyLayer);
        //RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + MovementDir * RaycastOffset, MovementDir, AttackDistance, 1<<PlayerLayer | 1<<EnemyLayer);
        if (hits.Length >0){
            RaycastHit hit = new RaycastHit();
            //RaycastHit2D hit = new RaycastHit2D();
            for (int i = 0; i < hits.Length; ++i){
                Unit unit = hits[i].collider.GetComponentInParent<Unit>();
                Tower tower = hits[i].collider.GetComponentInParent<Tower>();
                bool enemyDead = (unit != null && unit.Health == 0) || (tower != null && tower.Health == 0);
                if(hits[i].collider != null){
                    hit = hits[i];
                    if(hit.collider.gameObject.layer != EnemyTeamLayer) break;
                    if(!enemyDead) break;
                }
            }
            if (hit.collider != null){
                Unit unit = hit.collider.GetComponentInParent<Unit>();
                Tower tower = hit.collider.GetComponentInParent<Tower>();
                bool enemyDead = (unit != null && unit.Health == 0) || (tower != null && tower.Health == 0);

                if(hit.collider.gameObject.layer == EnemyTeamLayer && !enemyDead){
                    //Attack (Anim)
                    
                    
                    //Debug.Log("Atacooo: "+ hit.collider.gameObject + hit.collider.gameObject.transform.position);
                    Enemy = hit.collider.gameObject;
                    if(!atacando){
                        atacando = true;
                        StartCoroutine(AttackAction());
                    }   
                    //AttackAnim();
                }else{
                    Enemy = null;
                    atacando = false;
                    StopCoroutine(AttackAction());
                    float distance = Vector3.Distance(hit.point, (Vector3)(transform.position + MovementDir * RaycastOffset));
                    float stopDistance = StopDistance;
                    if(distance <= stopDistance && !enemyDead && tower == null){
                        IdleAnim();
                        //Idle (Anim)
                    }else{
                        //Move (Anim)
                        MoveAnim();
                        transform.position += MovementDir * Speed * Time.deltaTime;
                    }
                }
            }
        }else{
            //Move (Anim)
            MoveAnim();
            transform.position += MovementDir * Speed * Time.deltaTime;
        }

    }

    public void AddHealth(int amount)
    {
        Health = Mathf.Clamp(Health + amount, 0, InitialHealth);

        if (Health == 0)
        {
            // Dead
            StartCoroutine(Die());
            if (HealthBar.gameObject.activeSelf) HealthBar.gameObject.SetActive(false);
            // Effect
            if (amount < 0)
            {
                StartCoroutine(DamageEffect());
            }
            else
            {
                StartCoroutine(HealEffect());
            }
        }
        else if (Health == InitialHealth)
        {
            // Full life
            if (HealthBar.gameObject.activeSelf) HealthBar.gameObject.SetActive(false);
        }
        else
        {
            // Injured
            if (!HealthBar.gameObject.activeSelf) HealthBar.gameObject.SetActive(true);
            Vector3 localScale = HealthBar.rectTransform.localScale;
            localScale.x = (float)Health / InitialHealth;
            HealthBar.rectTransform.localScale = localScale;
            // Effect
            if (amount < 0)
            {
                StartCoroutine(DamageEffect());
            }
            else
            {
                StartCoroutine(HealEffect());
            }
        }
    }

    private IEnumerator Die()
    {
        if (!Dead)
        {
            Dead = true;
            //Die (Anim)
            DieAnim();
            BoxCollider collider = GetComponentInChildren<BoxCollider>();
            //if (collider != null) collider.enabled = false;
            Player.Base.colls.Remove(collider);
            if (collider != null) Destroy(collider);
            Player?.UnitKilled(gameObject, Type);
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }

    private WaitForSeconds WaitDamage = new WaitForSeconds(0.25f);
    private WaitForSeconds WaitAttack = new WaitForSeconds(AttackSpeed);
    private IEnumerator DamageEffect()
    {
        //Image.color = new Color(1.0f, ImageColor.g / 4.0f, ImageColor.b / 4.0f, ImageColor.a);
        yield return WaitDamage;
        //Image.color = ImageColor;
    }

    private IEnumerator HealEffect()
    {
        //Image.color = new Color(ImageColor.r / 4.0f, 1.0f, ImageColor.b / 4.0f, ImageColor.a);
        yield return WaitDamage;
        //Image.color = ImageColor;
    }

    private IEnumerator AttackAction(){
        yield return WaitAttack;
        Attack(Enemy);
        atacando=false;
    }

    public void OnAnimationAttackEnded()
    {
        if (Enemy != null)
        {
            Attack(Enemy);
        }
    }
    protected abstract void Attack(GameObject enemy);

    private void AttackAnim()
    {
        Animator?.SetBool(DieID, false);
        Animator?.SetBool(AttackID, true);
        Animator?.SetBool(IsWalkingID, false);
    }
    private void MoveAnim()
    {
        Animator?.SetBool(DieID, false);
        Animator?.SetBool(AttackID, false);
        Animator?.SetBool(IsWalkingID, true);
    }
    private void IdleAnim()
    {
        Animator?.SetBool(DieID, false);
        Animator?.SetBool(AttackID, false);
        Animator?.SetBool(IsWalkingID, false);
    }
    private void DieAnim()
    {
        Animator?.SetBool(DieID, true);
        Animator?.SetBool(AttackID, false);
        Animator?.SetBool(IsWalkingID, false);
    }


}
