using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : MonoBehaviour
{
    public static readonly int PlayerLayer = 8;
    public static readonly int EnemyLayer = 9;
    public bool IsPlayer;
    public string data;
    public static int popCost;
    public static int foodCost;
    public UnitType Type;
    public RawImage HealthBar;
    //Atributes
    public float Speed = 1.0f;
    public float AttackSpeed = 1.0f;
    public float AttackDistance = 0.5f;
    public float StopDistance = 0.5f;
    public int InitialHealth = 100;
    public float RaycastOffset = 0.1f;
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
    private int Health;
    private SpriteRenderer Image;
    private Color ImageColor;

    public enum UnitType{
        Soldier, Knight, Barbarian, Catapult
    }

    protected virtual void Awake()
    {
        Player = GetComponentInParent<Player>();
    }
    void Start()
    {
        //Animator = GetComponentInChildren<Animator>();
        //IsWalkingID = Animator.StringToHash("isWalking");
        //AttackID = Animator.StringToHash("attack");
        //DieID = Animator.StringToHash("die");
        MovementDir = IsPlayer ? Vector3.forward : Vector3.back;
        EnemyTeamLayer = IsPlayer ? EnemyLayer : PlayerLayer;
        //Animator.SetFloat("attackSpeed", AttackSpeed);

         Health = InitialHealth;
         foreach (Transform t in GetComponentsInChildren<Transform>()) t.gameObject.layer = IsPlayer? EnemyLayer : PlayerLayer;
         if (!IsPlayer)
        {
            Vector3 localScale = transform.localScale;
            localScale.z = -1;
            transform.localScale = localScale;
        }
    }

    
    void Update()
    {
        if (Health == 0) return;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + MovementDir * RaycastOffset, MovementDir, AttackDistance, 1<<PlayerLayer | 1<<EnemyLayer);
        if (hits.Length >0){
            RaycastHit2D hit = new RaycastHit2D();
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
                    Enemy = hit.collider.gameObject;
                }else{
                    Enemy = null;
                    float distance = Vector2.Distance(hit.point, (Vector2)(transform.position + MovementDir * RaycastOffset));
                    float stopDistance = StopDistance;
                    if(distance <= stopDistance && !enemyDead && tower == null){
                        //Idle (Anim)
                    }else{
                        //Move (Anim)
                        transform.position += MovementDir * Speed * Time.deltaTime;
                    }
                }
            }
        }else{
            //Move (Anim)
            transform.position += MovementDir * Speed * Time.deltaTime;
        }

    }

    public int getPopCost(){
        return popCost;
    }
    public int getFoodCost(){
        return foodCost;
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

        public bool Dead { get; private set; }
    private IEnumerator Die()
    {
        if (!Dead)
        {
            Dead = true;
            //Die (Anim)
            BoxCollider2D collider = GetComponentInChildren<BoxCollider2D>();
            if (collider != null) collider.enabled = false;
            Player?.UnitKilled(gameObject, Type);
            yield return new WaitForSeconds(2.0f);
            Destroy(gameObject);
        }
    }

    private WaitForSeconds WaitDamage = new WaitForSeconds(0.25f);
    private IEnumerator DamageEffect()
    {
        Image.color = new Color(1.0f, ImageColor.g / 4.0f, ImageColor.b / 4.0f, ImageColor.a);
        yield return WaitDamage;
        Image.color = ImageColor;
    }

    private IEnumerator HealEffect()
    {
        Image.color = new Color(ImageColor.r / 4.0f, 1.0f, ImageColor.b / 4.0f, ImageColor.a);
        yield return WaitDamage;
        Image.color = ImageColor;
    }

    public void OnAnimationAttackEnded()
    {
        if (Enemy != null)
        {
            Attack(Enemy);
        }
    }
    protected abstract void Attack(GameObject enemy);


}
