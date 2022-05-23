using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Tower : MonoBehaviour
{
    public event Action OnDestroyed;
    public event Action<int> OnDamaged;
    public Player Player;
    public bool dead = false;
    public int InitialHealth = 100;
    public RawImage HealthBar;
    public int Health { get; private set; }

    private Vector3 InitLocalPosition;
    //private SpriteRenderer Image;
    //private Color ImageColor;
    private void Awake() {
        InitLocalPosition = transform.localPosition;
        Reset();
    }
    public void Reset() {
        dead = false;
        Health = 50;
        //Debug.Log(Health.ToString() +" y "+ InitialHealth.ToString() );
        AddHealth(0);
        transform.localPosition = InitLocalPosition;
        StartCoroutine(UpdateLoop());
    }
    
    private WaitForSeconds WaitForHealth = new WaitForSeconds(0.2f);
    private IEnumerator UpdateLoop(){
         while(true){
            yield return WaitForHealth;
            Health = (int)Player.population;
            InitialHealth = (int)Player.popMax;
            UpdateBar();
         }
    }


    public void AddHealth(int amount)
    {
        int previousHealth = Health;
        Health = Mathf.Clamp(Health + amount, 0, InitialHealth);
        Player.addPop(amount);
        OnDamaged?.Invoke(previousHealth - Health);

        if (Health == 0)
        {
            // Dead
            dead=true;
            StartCoroutine(Die());
            if (HealthBar.gameObject.activeSelf) HealthBar.gameObject.SetActive(false);
            // Effect
            //StartCoroutine(DamageEffect());
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
            UpdateBar();
            // Effect
            //StartCoroutine(DamageEffect());
        }
    }

    private void UpdateBar(){
        Vector3 localScale = HealthBar.rectTransform.localScale;
        localScale.x = (float)Health / InitialHealth;
        HealthBar.rectTransform.localScale = localScale;
    }

    private IEnumerator Die()
    {
        Vector3 localPosition = transform.localPosition;
        while (localPosition.y > -2.5f)
        {
            localPosition.y -= Time.deltaTime * 2.0f;
            transform.localPosition = localPosition;
            yield return null;
        }
        OnDestroyed?.Invoke();
    }
    /*
    public int UnitsInTower { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Unit>() != null)
        {
            UnitsInTower += 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Unit>() != null)
        {
            UnitsInTower -= 1;
        }
    }
    
    private WaitForSeconds WaitDamage = new WaitForSeconds(0.25f);
    private IEnumerator DamageEffect()
    {
        //Image.color = new Color(1.0f, ImageColor.g / 4.0f, ImageColor.b / 4.0f, ImageColor.a);
        yield return WaitDamage;
        //Image.color = ImageColor;
    }
    */

}
