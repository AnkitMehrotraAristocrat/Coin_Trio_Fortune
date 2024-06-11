using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GameBackend.Data
{
    /// <summary>
    /// Used to create categorized random number queues, via SetCategoryQueues. 
    /// Fills framework random number queue when feature needs it, via ConsumeCategoryQueue.
    /// Retains queues across multiple spins, until SetCategoryQueues is called again.
    /// </summary>
    public class GaffeQueues
    {
        [JsonProperty] private Dictionary<GaffeCategories, Queue<Queue<ulong?>>> CategoryQueues { get; set; }
        private readonly Queue<GaffeCategories?> CategoryNames = new();

        public void Clear() {
            CategoryQueues = null;
            CategoryNames.Clear();
        }

        public bool HasCategoryQueues()
        {
            return CategoryQueues != null;
        }

        public void SetCategoryQueues(Queue<ulong?> readFrom)
        {
            // Will seperate the combined into categories
            // Categories will be as specified in GaffeCategoriesEnum
            // Only takes effect if readFrom starts with a valid category  
            if (readFrom == null || readFrom.Count == 0 || CategoryNames.Count == 0 || CategoryNames.Peek() == null) {
                CategoryQueues = null;
                CategoryNames.Clear();
                return;
            }
            CategoryQueues = new Dictionary<GaffeCategories, Queue<Queue<ulong?>>>();
            PopulateCategories(readFrom);
        }

        public void ConsumeCategoryQueue(GaffeCategories cat, Queue<ulong?> saveTo)
        {
            if (saveTo == null || CategoryQueues == null) {
                return;
            }

            saveTo.Clear();
            if (!CategoryQueues.ContainsKey(cat) || CategoryQueues[cat].Count == 0) {
                return;
            }

            var catValues = CategoryQueues[cat].Dequeue();
            do {
                saveTo.Enqueue(catValues.Dequeue());
            } while (catValues.Count != 0);
        }

        public void InterpretGaffeString(string value)
        {
            if (Enum.TryParse(value, true, out GaffeCategories category)) {
                CategoryNames.Enqueue(category);
            }
            else {
                CategoryNames.Enqueue(null);
            }
        }

        //////////////////////////////////////////////////////
        //////////////////////////////////////////////////////
        //////////////////////////////////////////////////////

        private void PopulateCategories(Queue<ulong?> combined)
        {
            do {
                var catNullable = CategoryNames.Dequeue();
                if (catNullable == null) {
                    continue;
                }
                combined.Dequeue(); //remove the null representing the category

                var cat = (GaffeCategories)catNullable;
                if (!CategoryQueues.ContainsKey(cat)) {
                    CategoryQueues.Add(cat, new Queue<Queue<ulong?>>());
                }

                var catValues = new Queue<ulong?>();
                CategoryQueues[cat].Enqueue(catValues);
                while (combined.Count != 0 && (combined.Peek() != null || CategoryNames.Peek() == null)) {
                    var val = combined.Dequeue();
                    catValues.Enqueue(val);
                }
            } while (CategoryNames.Count != 0);
        }
    }
}
