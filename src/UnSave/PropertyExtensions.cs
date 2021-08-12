using System;
using System.Collections.Generic;
using System.Linq;
using UnSave.Types;

namespace UnSave
{
    public static class PropertyExtensions {
        public static T FindProperty<T>(this IEnumerable<IUnrealProperty> properties, Func<IUnrealProperty, bool> predicate) where T : class, IUnrealProperty {
            return properties.FirstOrDefault(predicate) as T;
        }

        public static T FindProperty<T>(this GvasSaveData data, Func<IUnrealProperty, bool> predicate) where T : class, IUnrealProperty {
            return data.Properties.FindProperty<T>(predicate);
        }

        public static T FindProperty<T>(this IEnumerable<IUnrealProperty> properties, string name) where T : class, IUnrealProperty {
            return properties.FindProperty<T>(p => p.Name == name);
        }

        public static T FindProperty<T>(this GvasSaveData data, string name, bool ignoreCase = true) where T : class, IUnrealProperty {
            return ignoreCase ? data.Properties.FindProperty<T>(p => p.Name.ToLower() == name.ToLower()) : data.Properties.FindProperty<T>(p => p.Name == name);
        }

        public static IEnumerable<T> Items<T>(this UEArrayProperty arrayProp) {
            var matchingProps = arrayProp.Items.Where(i => i is T).Cast<T>();
            return matchingProps.Any() ? matchingProps : new List<T>();
        }
    }
}