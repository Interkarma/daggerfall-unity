#define DEBUG_SHOW_EDITDISTANCE_TIMES

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Utility
{
    public class EditDistance : IDistance
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

        private class InternalMatchResult : IComparable<InternalMatchResult>
        {
            public readonly string text;
            public readonly float distance;
            public readonly float relevance;

            public InternalMatchResult(string answer, float distance, float relevance)
            {
                this.text = answer;
                this.distance = distance;
                this.relevance = relevance;
            }

            public int CompareTo(InternalMatchResult otherResult)
            {
                if (otherResult == null) return 1;

                int compare = this.relevance.CompareTo(otherResult.relevance);
                if (compare == 0)
                    // Results will be returned in decreasing order
                    compare = string.Compare(otherResult.text, this.text, StringComparison.InvariantCulture);
                return compare;
            }

            public DistanceMatch GetMatchResult()
            {
                return new DistanceMatch(text, relevance);
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

        public float GetDistance(string s1, string s2, float upperBound = float.PositiveInfinity)
        {
            int l1 = s1.Length;
            int l2 = s2.Length;

            float[] row = new float[l2 + 1];
            for (int j = 0; j <= l2; j++)
                row[j] = seek_cost(s2, j);

            // row[j] is the distance between the empty string (0th prefix of s1) and the jth prefix of s2

            float[] future_row = new float[l2 + 1];

            for (int i = 0; i < l1; i++)
            {
                // row[j] is the distance between the ith prefix of s1 and the jth prefix of s2
                // future_row[j] should be the distance between the (i+1)th prefix of s1 and the jth prefix of s2

                // found_lesser: Is any cost in future_row[] lesser than upperBound?
                bool found_lesser = float.IsInfinity(upperBound);

                // j = 0, only available transition is deleting ith character of s1
                future_row[0] = row[0] + delete_cost(s1[i]);
                if (!found_lesser && future_row[0] <= upperBound)
                    found_lesser = true;

                for (int j1 = 0; j1 < l2; j1++) // j = j1 + 1
                {
                    if (s1[i] == s2[j1])
                        // Fast path: characters match, no extra cost
                        future_row[j1 + 1] = row[j1];
                    else
                    {
                        // Pick the cost of the cheapest transition available
                        future_row[j1 + 1] = Math.Min(
                                              Math.Min(row[j1 + 1] + delete_cost(s1[i]),
                                                       future_row[j1] + insert_cost(s2[j1])), 
                                              row[j1] + replace_cost(s1[i], s2[j1]));
                    }
                    if (!found_lesser && future_row[j1 + 1] <= upperBound)
                        found_lesser = true;
                }
                if (!found_lesser)
                    // Asuming all costs are positive, if no cost in future_row[] is lesser than upperBound, 
                    // then final result won't be either
                    return float.PositiveInfinity;

                float[] temp = row;
                row = future_row;
                future_row = temp;
            }

            // row[j] is the distance between s1 (l1th prefix of s1) and the jth prefix of s2

            float min = float.MaxValue;
            for (int j = 0; j <= l2; j++)
            {
                float cost = row[j] + trim_cost(s2, j);
                if (cost < min)
                    min = cost;
            }
            return min;
        }

        // Standard PriorityQueue, aka Heap datastructure
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

        private static float GetRelevance(float distance)
        {
            const float RelevanceDrop = 0.1f; // How fast relevance drops with distance
            return Mathf.Exp(-RelevanceDrop * distance);
        }

        public DistanceMatch[] FindBestMatches(string needle, int ntop)
        {
#if DEBUG_SHOW_EDITDISTANCE_TIMES
            // Start timing
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long startTime = stopwatch.ElapsedMilliseconds;
#endif

            string canonized_needle = canonize_string(needle);
            PriorityQueue<InternalMatchResult> kept = new PriorityQueue<InternalMatchResult>();
            float worseKeptDistance = float.PositiveInfinity;
            foreach (KeyValuePair<String, String> kv in dictionary)
            {
                float answer_distance = GetDistance(canonized_needle, kv.Value, worseKeptDistance);
                if (answer_distance < worseKeptDistance)
                {
                    kept.Enqueue(new InternalMatchResult(kv.Key, answer_distance, GetRelevance(answer_distance)));
                    // start dropping results?
                    if (kept.Count() > ntop)
                    {
                        kept.Dequeue();
                        worseKeptDistance = kept.Peek().distance;
                    }
                }
            }

            // Dump the heap in reverse order so highest relevances are at the beginning of results
            DistanceMatch[] result = new DistanceMatch[kept.Count()];
            for (int i = kept.Count(); i-- > 0; )
                result[i] = kept.Dequeue().GetMatchResult();

#if DEBUG_SHOW_EDITDISTANCE_TIMES
            // Show timer
            long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            DaggerfallUnity.LogMessage(string.Format("Time to findBestMatches {0}: {1}ms", needle, totalTime), true);
#endif
            return result;
        }
    }
}
