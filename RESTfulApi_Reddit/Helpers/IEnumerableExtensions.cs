using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Helpers {
    public static class IEnumerableExtensions {

        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source,
            string fields) {

            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            // create a list to hold our ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();


            // Reflection is expensive , so we do it once and reuse the results
            var propertyInfoList = new List<PropertyInfo>();


            if (string.IsNullOrWhiteSpace(fields)) {

                // all public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource)
                       .GetProperties(BindingFlags.IgnoreCase
                       | BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            }
            else {

                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit) {

                    var propertyName = field.Trim();


                    // use reflection to get the property on the source object
                    // we need to include public and instance, b/c specifying a binding 
                    // flag overwrites the already-existing binding flags.
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase |
                        BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null) {
                        throw new Exception($"Property {propertyName} wasn't found on" +
                            $"{typeof(TSource)}");
                    }

                    //add propertyInfo to list
                    propertyInfoList.Add(propertyInfo);
                }
            }
            foreach (TSource sourceObject in source) {

                // create an ExpandoObject that will hold the 
                // selected properties & values
                var dataShapedObject = new ExpandoObject();


                // Get the value of each property we have to return.  For that,
                // we run through the list
                foreach (var propertyInfo in propertyInfoList) {

                    // GetValue returns the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    //ExpandoObject is Dictionary. So we can add item to the dictionary
                    //Add the field to ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject)
                        .Add(propertyInfo.Name, propertyValue);
                }

                expandoObjectList.Add(dataShapedObject);
            }

            return expandoObjectList;
        }
    }
}
