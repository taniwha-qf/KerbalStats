using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

using KSP.IO;

namespace KerbalStats.Experience {
	class Experience
	{
		Dictionary<string, Task> tasks;
		HashSet<string> current;

		public void Load (ConfigNode node)
		{
			foreach (ConfigNode task_node in node.nodes) {
				tasks[task_node.name] = new Task ();
				tasks[task_node.name].Load (task_node);
			}
			var task_list = node.GetValue ("_current");
			if (task_list != null) {
				current.UnionWith (task_list.Split (','));
			}
		}

		public void Save (ConfigNode node)
		{
			foreach (var kv in tasks) {
				var task_node = new ConfigNode (kv.Key);
				node.AddNode (task_node);
				kv.Value.Save (task_node);
			}
			if (current.Count > 0) {
				var task_list = String.Join (",", current.ToArray ());
				node.AddValue ("_current", task_list);
			}
		}

		public Experience ()
		{
			tasks = new Dictionary<string, Task> ();
			current = new HashSet<string> ();
		}

		public void SetSituation (double UT, string situation)
		{
			foreach (var task in current) {
				tasks[task].SetSituation (UT, situation);
			}
		}

		public void BeginTask (double UT, string task, string situation)
		{
			if (!tasks.ContainsKey (task)) {
				tasks[task] = new Task ();
			}
			current.Add (task);
			tasks[task].BeginTask (UT, situation);
		}

		public void FinishTask (double UT, string task)
		{
			if (!tasks.ContainsKey (task)) {
				return;
			}
			current.Remove (task);
			tasks[task].FinishTask (UT);
		}

		public double GetExperience (double UT)
		{
			double exp = 0;
			foreach (var task in tasks.Values) {
				exp += task.Experience (UT);
			}
			return exp;
		}

		public double GetSituationExperience (double UT, string situation)
		{
			double exp = 0;
			foreach (var task in tasks.Values) {
				exp += task.Experience (UT, situation);
			}
			return exp;
		}

		public double GetTaskExperience (double UT, string task)
		{
			double exp = 0;
			if (tasks.ContainsKey (task)) {
				exp += tasks[task].Experience (UT);
			}
			return exp;
		}

		public double GetTaskExperience (double UT, string task, string situation)
		{
			double exp = 0;
			if (tasks.ContainsKey (task)) {
				exp += tasks[task].Experience (UT, situation);
			}
			return exp;
		}
	}
}