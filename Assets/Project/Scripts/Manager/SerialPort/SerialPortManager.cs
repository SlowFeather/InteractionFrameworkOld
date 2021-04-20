using InteractionFramework.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
namespace InteractionFramework.Runtime
{
	public class SerialPortManager : MonoSingletion<SerialPortManager>
	{
		#region 串口参数,主要修改串口名与波特率
		/// <summary>
		/// 串口名
		/// </summary>
		public string portName = "COM5";
		/// <summary>
		/// 波特率
		/// </summary>
		public int baudRate = 115200;
		private Parity parity = Parity.None;
		private int dataBits = 8;
		private StopBits stopBits = StopBits.One;
		SerialPort sp = null;
		#endregion

		#region 消息处理相关
		/// <summary>
		/// 缓存消息列表
		/// </summary>
		List<byte> bufferList = new List<byte>();
		/// <summary>
		/// 一条消息的长度
		/// </summary>
		int messageLen = 1;
		#endregion

		public int state = 0;
		// Start is called before the first frame update
		void Start()
		{
			IniStorage.mINIFileName = Application.streamingAssetsPath + "/IFConfig/config.ini";

			//IniStorage.WriteString("PortName",IniStorage.SectionName.config,"COM1");
			//IniStorage.WriteFloat("BaudRate", IniStorage.SectionName.config, 9600);

		}

		public void InitSerialPort()
		{
			portName = IniStorage.GetString("PortName");
			baudRate = IniStorage.GetInt("BaudRate");


			OpenPort(portName, baudRate);
			StartCoroutine(DataReceiveFunction());
		}

		/// <summary>
		/// 接收数据协程
		/// </summary>
		/// <returns></returns>
		IEnumerator DataReceiveFunction()
		{

			while (true)
			{
				if (sp != null && sp.IsOpen)
				{
					try
					{
						RecAndProcessingFunction();
					}
					catch (Exception ex)
					{
						Debug.LogError(ex);
					}
				}
				yield return new WaitForSeconds(Time.deltaTime);
			}
		}
		/// <summary>
		/// 读取并处理消息
		/// </summary>
		private void RecAndProcessingFunction()
		{
			//待读字节个数
			int n = sp.BytesToRead;
			//创建n个字节的缓存
			byte[] buf = new byte[n];
			//读到在数据存储到buf
			sp.Read(buf, 0, n);
			//1.缓存数据 不断地将接收到的数据加入到buffer链表中
			bufferList.AddRange(buf);
			//2.完整性判断 至少包含帧头（1字节）、类型（1字节）、功能位（22字节） 根据设计不同而不同
			while (bufferList.Count >= 1)
			{
				//得到一帧完整的数据，进行处理，在此之前可以使用校验位保证此帧数据完整性
				byte[] processingByteArray = new byte[messageLen];
				//从缓存池中拷贝到处理数组
				bufferList.CopyTo(0, processingByteArray, 0, messageLen);
				//处理一帧数据
				//Debug.Log(byteToHexStr(processingByteArray));
				DataProcessingFunction(processingByteArray);
				bufferList.RemoveRange(0, messageLen);
			}
		}
		/// <summary>
		/// 数据处理
		/// </summary>
		private void DataProcessingFunction(byte[] dataBytes)
		{
			int m = int.Parse(System.Text.ASCIIEncoding.Default.GetString(dataBytes));
			state = m;
		}

		#region 串口开启关闭相关
		//打开串口
		public void OpenPort(string DefaultPortName, int DefaultBaudRate)
		{
			sp = new SerialPort(DefaultPortName, DefaultBaudRate, parity, dataBits, stopBits);
			sp.ReadTimeout = 10;
			try
			{
				if (!sp.IsOpen)
				{
					sp.Open();

				}
			}

			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}

		//关闭串口
		public void ClosePort()
		{
			try
			{
				sp.Close();
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}
		#endregion

		#region Unity
		private void OnApplicationQuit()
		{
			ClosePort();
		}
		private void OnDisable()
		{
			ClosePort();
		}
		#endregion



		/// <summary>
		/// 字节数组转16进制字符串
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string byteToHexStr(byte[] bytes)
		{
			string returnStr = "";
			if (bytes != null)
			{
				for (int i = 0; i < bytes.Length; i++)
				{
					returnStr += bytes[i].ToString("X2");
					returnStr += "-";
				}
			}
			return returnStr;
		}
	}
}