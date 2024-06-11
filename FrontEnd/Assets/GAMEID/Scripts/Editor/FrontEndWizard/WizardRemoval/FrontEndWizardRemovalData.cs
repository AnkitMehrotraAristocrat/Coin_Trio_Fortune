using PixelUnited.NMG.Slots.Milan.Wizard;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WizardRemovalData", menuName = "NMG/Wizard/Wizard Removal Data")]
public class FrontEndWizardRemovalData : ScriptableObject
{
    [SerializeField] private List<MechanicConfiguration> _mechanicConfigurations;
    public List<MechanicConfiguration> MechanicConfigurations => _mechanicConfigurations;
}