using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace Asv.Avalonia.GMap
{
    


    public class MapShapeDataTemplateSelector:IDataTemplate
    {
        [Content]
        public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = new();

        public IControl Build(object param)
        {
            if (param is MapShapeViewModel shape)
            {
                var key = shape.ShapeType;
                return AvailableTemplates[key.ToString()].Build(param);
            }

            return null;
        }

        public bool Match(object data)
        {
            // Our Keys in the dictionary are strings, so we call .ToString() to get the key to look up
            if (data is MapShapeViewModel shape)
            {
                var key = shape.ShapeType;
                return AvailableTemplates.ContainsKey(key.ToString());
            }

            return false;
        }
    }
}