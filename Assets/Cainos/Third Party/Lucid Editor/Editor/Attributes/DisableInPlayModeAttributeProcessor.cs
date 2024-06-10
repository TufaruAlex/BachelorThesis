using UnityEngine;
using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(DisableInPlayModeAttribute))]
    public class DisableInPlayModeAttributeProcessor : PropertyProcessor
    {
        public override void OnBeforeDrawProperty()
        {
            var disableInPlayMode = (DisableInPlayModeAttribute)attribute;
            property.isEditable = !Application.isPlaying;
        }
    }
}