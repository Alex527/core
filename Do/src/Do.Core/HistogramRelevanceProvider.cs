/* HistogramRelevanceProvider.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * inner distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;

using System.IO;
using System.Threading;

using Do.Universe;

namespace Do.Core
{
	class HistogramRelevanceProvider : RelevanceProvider {
		
		int maxActionHits, maxItemHits;
		Dictionary<int, int> itemHits, actionHits;
		
		Timer serializeTimer;
		const int SerializeInterval = 10*60;
		
		static bool LetterOccursAfterDelimiter (string s, char a)
		{
			int idx;

			idx = 0;
			while (idx < s.Length && (idx = s.IndexOf (a, idx)) > -1) {
				if (idx == 0 ||
					(idx > 0 && s[idx-1] == ' ')) {
					return true;
				}
				idx++;
			}
			return false;
		}

		public HistogramRelevanceProvider ()
		{
			maxActionHits = maxItemHits = 0;
			itemHits = new Dictionary<int,int> ();
			actionHits = new Dictionary<int,int> ();

			Deserialize ();
			
			// Serialize every few minutes.
			serializeTimer = new Timer (OnSerializeTimer);
			serializeTimer.Change (SerializeInterval*1000, SerializeInterval*1000);
		}
		
		protected string DB {
			get {
				return "~/.do/relevance2".Replace ("~",
					Environment.GetFolderPath (Environment.SpecialFolder.Personal));
			}
		}
		
		private void OnSerializeTimer (object state)
		{
			Serialize ();
		}
			
		protected void Deserialize ()
		{
			lock (itemHits)
			lock (actionHits) {
				try {
					Log.Info ("Deserializing HistogramRelevanceProvider...");
					bool isAction;
					string[] parts;
					int	key, value;
					foreach (string line in File.ReadAllLines (DB)) {
						try {
							parts = line.Split ('\t');
							key = int.Parse (parts[0]);
							value = int.Parse (parts[1]);
							isAction = parts[2] == "1";

							if (isAction) {
								actionHits[key] = value;
								maxActionHits = Math.Max (maxActionHits, value);
							} else {
								itemHits[key] = value;
								maxItemHits = Math.Max (maxItemHits, value);
							}
						} catch {
							continue;
						}
					}
					Log.Info ("Successfully deserialized HistogramRelevanceProvider.");
				} catch (Exception e) {
					Log.Error ("Deserializing HistogramRelevanceProvider failed: {0}", e.Message);
				}
			}
		}
		
		protected void Serialize ()
		{
			lock (itemHits)
			lock (actionHits) {
				try {
					Log.Info ("Serializing HistogramRelevanceProvider...");
					using (StreamWriter writer = new StreamWriter (DB)) {
						// Serialize item hits information:
						foreach (KeyValuePair<int, int> kvp in itemHits) {
							writer.WriteLine (string.Format ("{0}\t{1}\t0", kvp.Key, kvp.Value));
						}
						// Serialize action hits information:
						foreach (KeyValuePair<int, int> kvp in actionHits) {
							writer.WriteLine (string.Format ("{0}\t{1}\t1", kvp.Key, kvp.Value));
						}
					}
					Log.Info ("Successfully serialized HistogramRelevanceProvider.");
				} catch (Exception e) {
					Log.Error ("Serializing HistogramRelevanceProvider failed: {0}", e.Message);
				}
			}
		}

		public override bool CanBeFirstResultForKeypress (DoObject r, char a)
		{
			return LetterOccursAfterDelimiter (r.Name.ToLower (), char.ToLower (a));
		}

		public override void IncreaseRelevance (DoObject r, string match, DoObject other)
		{
			int rel;
			int key = r.GetHashCode ();
			Dictionary<int, int> hits;
			int maxHits;
			
			if (r is DoTextItem) return;

			if (!(r is IAction)) {
				hits = itemHits;
				maxHits = maxItemHits;
			} else {
				hits = actionHits;
				maxHits = maxActionHits;
			}

			lock (hits) {
				hits.TryGetValue (key, out rel);
				rel = rel + 1;
				hits[key] = rel;
				maxHits = Math.Max (maxHits, rel);
			}

			if (!(r is IAction))
				maxItemHits = maxHits;
			else
				maxActionHits = maxHits;
		}
		
		public override void DecreaseRelevance (DoObject r, string match, DoObject other)
		{
			int rel;
			int key = r.GetHashCode ();
			Dictionary<int, int> hits;
			
			if (r is DoTextItem) return;

			if (!(r is IAction)) {
				hits = itemHits;
			} else {
				hits = actionHits;
			}

			lock (hits) {
				hits.TryGetValue (key, out rel);
				rel = rel - 1;
				if (rel == 0) {
					hits.Remove (key);
				} else {
					hits[key] = rel;
				}
			}
		}
		
		public override float GetRelevance (DoObject r, string match, DoObject other)
		{
			float relevance, score, itemReward; // These should all be between 0 and 1.
			int key = r.GetHashCode ();
			Dictionary<int, int> hits;
			int objectHits, maxHits;

			score = base.GetRelevance (r, match, other);
			if (score == 0) return 0;

			if (!(r is IAction)) {
				hits = itemHits;
				maxHits = maxItemHits;
				itemReward = 1f;
			} else {
				hits = actionHits;
				maxHits = maxActionHits;
				itemReward = 0f;
			}

			lock (hits) {
				hits.TryGetValue (key, out objectHits);
			}
			if (objectHits == 0)
				relevance = 0f;
			else
				relevance = (float) objectHits / (float) maxHits;

			/*
			if (match == "")
			Console.WriteLine ("{0}: {1} has relevance {2} ({3}/{4}) and score {5}.",
				match, r, relevance, objectHits, maxHits, score);
			*/

			return itemReward * .20f +
				   relevance  * .30f +
			       score      * .70f;
		}
	}
}