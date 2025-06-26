using UnityEngine;

public class ParticleCircleActivator : MonoBehaviour
{
    public GameObject panel; // O painel que controla a ativação
    public ParticleSystem circleParticles; // Sistema de partículas

    void Start()
    {
        // Se não for atribuído manualmente, procura nas children
        if (circleParticles == null)
        {
            circleParticles = GetComponentInChildren<ParticleSystem>();
        }

        // Garante que começa desativado
        if (circleParticles != null)
        {
            circleParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    void Update()
    {
        if (panel == null || circleParticles == null) return;

        if (panel.activeSelf && !circleParticles.isPlaying)
        {
            circleParticles.Play();
        }
        else if (!panel.activeSelf && circleParticles.isPlaying)
        {
            // Fade-out suave
            circleParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
