using Global.View;

namespace Global.Controller
{
    public class LayersController
    {
        private readonly LayersProperties _layersProperties;
        public LayersController(LayersProperties layersProperties)
        {
            _layersProperties = layersProperties;
        }

        public LayersProperties.LayerProperties GetLayerInfo(LayersProperties.ItemName itemName)
        {
            return _layersProperties[itemName];
        }
    }
}
