using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CheckboxSliderControl : MonoBehaviour
{
    [SerializeField]
    private List<Slider> slidersToBlock;  // Lista de sliders que você vai definir no Unity Inspector
    [SerializeField]
    private Toggle checkbox;  // A checkbox que vai controlar os sliders

    private void Start()
    {
        // Inicializar o estado dos sliders com base no estado da checkbox
        UpdateSliderState(checkbox.isOn);

        // Adicionar o ouvinte de mudança de estado na checkbox
        checkbox.onValueChanged.AddListener(UpdateSliderState);
    }

    // Método que atualiza o estado dos sliders com base no valor da checkbox
    private void UpdateSliderState(bool isChecked)
    {
        foreach (Slider slider in slidersToBlock)
        {
            if (slider != null)
                slider.interactable = !isChecked;  // Se a checkbox estiver ativada, bloqueia o slider
        }
    }
}
