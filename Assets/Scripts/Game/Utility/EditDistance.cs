using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Utility
{
    public class EditDistance
    {
        private readonly Func<string, string> canonize_string;
        private readonly Func<char, float> insert_cost;
        private readonly Func<char, float> delete_cost;
        private readonly Func<char, char, float> replace_cost;
        private readonly Func<string, int, float> seek_cost;
        private readonly Func<string, int, float> trim_cost;

        // key: display string
        // value: canonized string
        private SortedDictionary<String, String> dictionary;

        public class MatchResult : IComparable<MatchResult>
        {
            public readonly string text;
            public readonly float distance;
            public readonly float relevance;

            public MatchResult(string answer, float distance, float relevance)
            {
                this.text = answer;
                this.distance = distance;
                this.relevance = relevance;
            }

            public int CompareTo(MatchResult otherResult)
            {
                if (otherResult == null) return 1;

                int compare = this.relevance.CompareTo(otherResult.relevance);
                if (compare == 0)
                    compare = string.Compare(this.text, otherResult.text, StringComparison.InvariantCulture);
                return compare;
            }
        }

        public EditDistance(Func<string, string> canonize_string,
                            Func<char, float> insert_cost,
                            Func<char, float> delete_cost,
                            Func<char, char, float> replace_cost,
                            Func<string, int, float> seek_cost,
                            Func<string, int, float> trim_cost
                            )
        {
            this.canonize_string = canonize_string;
            this.insert_cost = insert_cost;
            this.delete_cost = delete_cost;
            this.replace_cost = replace_cost;
            this.seek_cost = seek_cost;
            this.trim_cost = trim_cost;
        }

        private class KeyComparer : Comparer<KeyValuePair<String, String>>
        {
            public override int Compare(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
            {
                return string.Compare(x.Key, y.Key, StringComparison.InvariantCulture);
            }
        }

        public void SetDictionary(String[] dictionary)
        {
            SetDictionary(new List<String>(dictionary));
        }

        public void SetDictionary(List<String> dictionary)
        {
            this.dictionary = new SortedDictionary<string, string>();
            foreach (string s in dictionary)
            {
                // Silently deduplicates keys, whereas Add() would throw an exception
                this.dictionary[s] = canonize_string(s);
            }
        }

        public float distance(string s1, string s2, float upperBound = float.PositiveInfinity)
        {
            int l1 = s1.Length;
            int l2 = s2.Length;
            float[] row = new float[l2 + 1];
            for (int j = 0; j <= l2; j++)
                row[j] = seek_cost(s2, j);
            float[] future_row = new float[l2 + 1];
            for (int i = 0; i < l1; i++)
            {
                future_row[0] = row[0] + delete_cost(s1[i]);
                bool found_lower = float.IsInfinity(upperBound);
                for (int j = 0; j < l2; j++)
                {
                    if (s1[i] == s2[j])
                        future_row[j+1] = row[j];
                    else
                    {
                        float cost = Math.Min(future_row[j] + insert_cost(s2[j]),
                                                    row[j+1] + delete_cost(s1[i]));
                        future_row[j+1] = Math.Min(cost, row[j] + replace_cost(s1[i], s2[j]));
                    }
                    if (!found_lower && future_row[j + 1] < upperBound)
                        found_lower = true;
                }
                if (!found_lower)
                    return float.PositiveInfinity;

                float[] temp = row;
                row = future_row;
                future_row = temp;
            }
            if (l2 > l1)
            {
                float min = float.MaxValue;
                for (int j = 0; j <= l2; j++)
                {
                    float cost = row[j] + trim_cost(s2, j);
                    if (cost < min)
                        min = cost;
                }
                return min;
            }
            else
                return row[l2];
        }

        private class PriorityQueue<T> where T : IComparable<T>
        {
            private List<T> data;

            public PriorityQueue()
            {
                this.data = new List<T>();
            }

            public void Enqueue(T item)
            {
                data.Add(item);
                int child_index = data.Count - 1;
                while (child_index > 0)
                {
                    int parent_index = (child_index - 1) / 2;
                    if (data[child_index].CompareTo(data[parent_index]) >= 0) 
                        break;
                    T tmp = data[child_index]; 
                    data[child_index] = data[parent_index]; 
                    data[parent_index] = tmp;
                    child_index = parent_index;
                }
            }

            public T Dequeue()
            {
                if (data.Count == 0)
                    throw new InvalidOperationException("PriorityQueue is empty");

                int last_index = data.Count - 1;
                T front_item = data[0];
                data[0] = data[last_index];
                data.RemoveAt(last_index);

                --last_index;
                int parent_index = 0;
                while (true)
                {
                    int left_child_index = parent_index * 2 + 1;
                    if (left_child_index > last_index) 
                        break;
                    int right_child_index = left_child_index + 1;
                    if (right_child_index <= last_index && data[right_child_index].CompareTo(data[left_child_index]) < 0)
                        left_child_index = right_child_index;
                    if (data[parent_index].CompareTo(data[left_child_index]) <= 0) 
                        break;
                    T tmp = data[parent_index]; 
                    data[parent_index] = data[left_child_index]; 
                    data[left_child_index] = tmp;
                    parent_index = left_child_index;
                }
                return front_item;
            }

            public T Peek()
            {
                if (data.Count == 0)
                    throw new InvalidOperationException("PriorityQueue is empty");

                return data[0];
            }

            public int Count()
            {
                return data.Count;
            }
        }

        public MatchResult[] FindBestMatches(string needle, int ntop)
        {
            string canonized_needle = canonize_string(needle);
            PriorityQueue<MatchResult> kept = new PriorityQueue<MatchResult>();
            float worseKeptDistance = float.PositiveInfinity;
            foreach (KeyValuePair<String, String> kv in dictionary)
            {
                float answer_distance = distance(canonized_needle, kv.Value, worseKeptDistance);
                if (answer_distance < worseKeptDistance)
                {
                    // use opposite of distance as relevance so low distances give highest relevances
                    kept.Enqueue(new MatchResult(kv.Key, answer_distance, -answer_distance));
                    // start dropping results?
                    if (kept.Count() > ntop)
                    {
                        kept.Dequeue();
                        worseKeptDistance = kept.Peek().distance;
                    }
                }
            }

            // Dump the heap in reverse order so highest relevances are at the beginning of results
            MatchResult[] result = new MatchResult[kept.Count()];
            for (int i = kept.Count(); i-- > 0; )
                result[i] = kept.Dequeue();
            return result;
        }
    }
}
