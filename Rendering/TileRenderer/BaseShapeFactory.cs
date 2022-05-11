using Mapster.Common.ShapeTypes;
using Mapster.Common.MemoryMappedTypes;

namespace Mapster.Rendering
{
    internal class BaseShapeFactory
    {
        public static BaseShape CreateShape(ShapeType type, MapFeatureData feature)
        {
            BaseShape baseShape = null;
            switch (type)
            {
                case ShapeType.Road:
                    baseShape = new Road(feature.Coordinates);
                    break;
                case ShapeType.Waterway:
                    baseShape = new Waterway(feature.Coordinates, feature.Type == GeometryType.Polygon);
                    break;
                case ShapeType.Border:
                    baseShape = new Border(feature.Coordinates);
                    break;
                case ShapeType.PopulatedPlace:
                    baseShape = new PopulatedPlace(feature.Coordinates, feature);
                    break;
                case ShapeType.Railway:
                    baseShape = new Railway(feature.Coordinates);
                    break;
                case ShapeType.Natural:
                    baseShape = new GeoFeature(feature.Coordinates, feature);
                    break;
                case ShapeType.Forest:
                    baseShape = new GeoFeature(feature.Coordinates, GeoFeature.GeoFeatureType.Forest);
                    break;
                case ShapeType.Residential:
                    baseShape = new GeoFeature(feature.Coordinates, GeoFeature.GeoFeatureType.Residential);
                    break;
                case ShapeType.Plain:
                    baseShape = new GeoFeature(feature.Coordinates, GeoFeature.GeoFeatureType.Plain);
                    break;
                case ShapeType.Water:
                    baseShape = new GeoFeature(feature.Coordinates, GeoFeature.GeoFeatureType.Water);
                    break;
            }
            return baseShape;
        }
    }
}
