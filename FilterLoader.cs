//Copyright 2012 Joshua Scoggins. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are
//permitted provided that the following conditions are met:
//
//   1. Redistributions of source code must retain the above copyright notice, this list of
//      conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright notice, this list
//      of conditions and the following disclaimer in the documentation and/or other materials
//      provided with the distribution.
//
//THIS SOFTWARE IS PROVIDED BY Joshua Scoggins ``AS IS'' AND ANY EXPRESS OR IMPLIED
//WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Joshua Scoggins OR
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//The views and conclusions contained in the software and documentation are those of the
//authors and should not be interpreted as representing official policies, either expressed
//or implied, of Joshua Scoggins. 
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using Frameworks.Plugin;
using Libraries.Messaging;
using Libraries.Collections;
using System.IO;

namespace Libraries.Filter
{
	public class Initiator : MarshalByRefObject, IPluginLoader 
	{
		private FilterStreamInterpreter interpreter;
		private MemoryStream communications;
		public MemoryStream Communications { get { return communications; } }
		Stream IPluginLoader.CommunicationStream { get { return communications; } }
		private Tuple<string, string, Guid>[] boundaryTuples;
		public Tuple<string, string, Guid>[] DesiredPluginInformation { get { return boundaryTuples; } }
		private Dictionary<Guid, Guid> translationLayer;
		public Initiator(string[] paths)
		{
			communications = new MemoryStream();
			interpreter = new FilterStreamInterpreter(communications);
			translationLayer = new Dictionary<Guid, Guid>(); 
			List<Tuple<string, string, Guid>> r = new List<Tuple<string, string, Guid>>();
			foreach(string str in paths)
			{
				var guids = FilterLoader.LoadPlugins(str);
				foreach(var guid in guids.Item2)
				{
					translationLayer[guid] = guids.Item1;
					r.Add(FilterLoader.GetPlugin(guids.Item1, guid));
				}
			}
			boundaryTuples = r.ToArray();
		}

		public Message Invoke(Message input)
		{
			long startingPosition = (long)input.Value;
			communications.Position = startingPosition;
			Hashtable ht = interpreter.CreateData();
			Message m2 = new Message(input.Sender, input.Receiver, input.OperationType,
					ht);
			Message oMSG = FilterLoader.Invoke(translationLayer[input.Receiver], m2);
			Hashtable table = (Hashtable)oMSG.Value;
			if(table.ContainsKey("terminate"))
			{
				communications.Position = 0L;
				TransmissionEncoder.PutIntoHashtable("terminate", table["terminate"].ToString(),
						communications);
				TransmissionEncoder.WriteCode((byte)255, communications);
				communications.Position = 0L;
				return new Message(Guid.NewGuid(), oMSG.ObjectID, oMSG.Sender, 
						MessageOperationType.Return,
						false); 
			}
			else
			{
				communications.Position = 0L;
				TransmissionEncoder.PutIntoHashtable("width", (int)table["width"], communications);
				TransmissionEncoder.PutIntoHashtable("height", (int)table["height"], communications);
				TransmissionEncoder.PutIntoHashtable("result", table["result"].ToString(), communications);
				TransmissionEncoder.WriteCode((byte)255, communications);
				communications.Position = 0L;
				return new Message(Guid.NewGuid(), oMSG.ObjectID, oMSG.Sender, 
						MessageOperationType.Return,
						true); 
			}

		}
	}
	public static class FilterLoader
	{
		private static Dictionary<Guid, PluginLoader> pluginEnvironments;
		static FilterLoader()
		{
			pluginEnvironments = new Dictionary<Guid, PluginLoader>();
		}
		public static Message Invoke(Guid targetPluginGroup, Message input)
		{
			return pluginEnvironments[targetPluginGroup].Invoke(input);
		}
		public static Tuple<string, string, Guid> GetPlugin(Guid targetGuid, Guid pluginGuid)
		{
			Filter target = (Filter)pluginEnvironments[targetGuid][pluginGuid];
			return new Tuple<string, string, Guid>(target.Name, target.InputForm, target.ObjectID);
		}
		public static Tuple<Guid, Guid[]> LoadPlugins(string path)
		{
			PluginLoader pl = new PluginLoader(path);
			pluginEnvironments.Add(pl.ObjectID, pl); 
			return new Tuple<Guid,Guid[]>(pl.ObjectID, pl.Names.ToArray());
		}
	}
}
