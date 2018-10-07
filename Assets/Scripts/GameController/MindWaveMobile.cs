using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class MindWaveMobile : MonoBehaviour {

    private TcpClient client;
    private Stream stream;
    private byte[] buffer;

    public bool reading = false;
    public float readInFrequency = .08f;

    public delegate void UpdateIntValueDelegate(int value);
    public delegate void UpdateFloatValueDelegate(float value);

    public event UpdateIntValueDelegate UpdatePoorSignalEvent;
    public event UpdateIntValueDelegate UpdateAttentionEvent;
    public event UpdateIntValueDelegate UpdateMeditationEvent;

    // Connect to the EEG device
    public void Connect() {
        if (!reading) {
            reading = true;
            client = new TcpClient("127.0.0.1", 13854);
            stream = client.GetStream();
            buffer = new byte[1024];
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": true, ""format"": ""Json""}");
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);

            InvokeRepeating("ParseData", 0.1f, 0.08f);
        }
    }

    public void Disconnect() {
        if (IsInvoking("ParseData")) {
            CancelInvoke("ParseData");
            stream.Close();
        }
    }

    void ParseData() {
        if (stream.CanRead) {
            try {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                string[] packets = Encoding.ASCII.GetString(buffer, 0, bytesRead).Split('\r');

                foreach (string packet in packets) {
                    if (packet.Length == 0)
                        continue;

                    IDictionary primary = (IDictionary)JsonConvert.Import(typeof(IDictionary), packet);

                    if (primary.Contains("poorSignalLevel")) {

                        if (UpdatePoorSignalEvent != null) {
                            UpdatePoorSignalEvent(int.Parse(primary["poorSignalLevel"].ToString()));
                        }

                        if (primary.Contains("eSense")) {
                            IDictionary eSense = (IDictionary)primary["eSense"];
                            if (UpdateAttentionEvent != null) {
                                UpdateAttentionEvent(int.Parse(eSense["attention"].ToString()));
                            }
                            if (UpdateMeditationEvent != null) {
                                UpdateMeditationEvent(int.Parse(eSense["meditation"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (IOException e) { Debug.Log("IOException " + e); }
            catch (System.Exception e) { Debug.Log("Exception " + e); }
        }
    }
   
    // Disconnect if the game is exited
    void OnApplicationQuit() {
        Disconnect();
    }
}
