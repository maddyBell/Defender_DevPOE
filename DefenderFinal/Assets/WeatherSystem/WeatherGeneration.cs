using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeatherGeneration : MonoBehaviour
{
     public enum WeatherType
    {
        Sunny,
        Rain,
        Fog,

    }
    public float minWeatherDuration = 15f;
    public float maxWeatherDuration = 25f;
    public float transitionTime = 3f; 

    public ParticleSystem rainEffect;
    public ParticleSystem fogEffect;

    public GameObject rainPrefab;
    public GameObject fogPrefab;

    public float rainSlowMultiplier = 0.8f;
    public float fogRangeMultiplier = 0.5f;
    public WeatherType currentWeather;

    private WeatherType targetWeather;

    private float rainMultiplier = 1f;
    private float fogMultiplier = 1f;

    private void Start()
    {
        StartCoroutine(WeatherCycle());
    }

    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            //choosing weather 
            targetWeather = (WeatherType)Random.Range(0, System.Enum.GetValues(typeof(WeatherType)).Length);
            yield return StartCoroutine(TransitionWeather(targetWeather));
            Debug.Log("Current Weather: " + targetWeather);
            // waiting to change
            float duration = Random.Range(minWeatherDuration, maxWeatherDuration);
            yield return new WaitForSeconds(duration);
        }
    }

    private IEnumerator TransitionWeather(WeatherType newWeather)
    {
        WeatherType previousWeather = currentWeather;
        currentWeather = newWeather;
        ParticleSystem effect;

        //play the particles
        if (newWeather == WeatherType.Rain)
        {
            effect = MakeRain();
        }
        else if (newWeather == WeatherType.Fog)
        {
            effect = MakeFog();
        }
        else
        {
            effect = null;
        }

        ParticleSystem newEffect = effect;
       //storing starting values
        float startRain = rainMultiplier;
        float startFog = fogMultiplier;


        // Setting new target values
        float targetRain = (newWeather == WeatherType.Rain) ? rainSlowMultiplier : 1f;
        float targetFog = (newWeather == WeatherType.Fog) ? fogRangeMultiplier : 1f;


        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionTime;

            // changes gameplay values
            rainMultiplier = Mathf.Lerp(startRain, targetRain, t);
            fogMultiplier = Mathf.Lerp(startFog, targetFog, t);
  
            yield return null;
        }

        // setting finals
        rainMultiplier = targetRain;
        fogMultiplier = targetFog;

        // Stopping old weather
        ParticleSystem oldEffect = GetEffect(previousWeather);
        if (oldEffect != null && oldEffect != newEffect)
        {
            oldEffect.Stop();
        }

        Debug.Log("Stopped Effect: " + oldEffect);
        
    }

    private ParticleSystem GetEffect(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Rain: return rainEffect;
            case WeatherType.Fog: return fogEffect;

            default: return null;
        }
    }

    public ParticleSystem MakeRain()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
             GameObject rain = Instantiate(rainPrefab, player.fogOrigin.position, Quaternion.identity, player.fogOrigin);
            rainEffect.Play();
        }
        return rainEffect;
    }

    public ParticleSystem MakeFog()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            GameObject fog = Instantiate(fogPrefab, player.fogOrigin.position, Quaternion.identity, player.fogOrigin);
            fogEffect.Play();
        }
        return fogEffect;
    }

    // making public
    public float GetRainMultiplier() => rainMultiplier;
    public float GetFogMultiplier() => fogMultiplier;

}
