// ManagedNvApi, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// ManagedNvApi.NvApi
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using _003CCppImplementationDetails_003E;
using ManagedNvApi;
using sidle;

public class NvApi : IDisposable
{
	public enum CHANNEL_ID
	{
		CHANNEL_ID_TOTAL_GPU_POWER,
		CHANNEL_ID_NORMALIZED_TOTAL_POWER
	}

	public enum NV_GPU_CLIENT_ILLUM_ZONE_TYPE
	{
		NV_GPU_CLIENT_ILLUM_ZONE_TYPE_INVALID,
		NV_GPU_CLIENT_ILLUM_ZONE_TYPE_RGB,
		NV_GPU_CLIENT_ILLUM_ZONE_TYPE_COLOR_FIXED,
		NV_GPU_CLIENT_ILLUM_ZONE_TYPE_RGBW,
		NV_GPU_CLIENT_ILLUM_ZONE_TYPE_SINGLE_COLOR
	}

	public enum NV_GPU_CLIENT_ILLUM_ZONE_LOCATION : uint
	{
		NV_GPU_CLIENT_ILLUM_ZONE_LOCATION_GPU_TOP_0 = 0u,
		NV_GPU_CLIENT_ILLUM_ZONE_LOCATION_GPU_FRONT_0 = 8u,
		NV_GPU_CLIENT_ILLUM_ZONE_LOCATION_GPU_BACK_0 = 12u,
		NV_GPU_CLIENT_ILLUM_ZONE_LOCATION_SLI_TOP_0 = 32u,
		NV_GPU_CLIENT_ILLUM_ZONE_LOCATION_INVALID = uint.MaxValue
	}

	public enum NV_GPU_CLIENT_ILLUM_PIECEWISE_LINEAR_CYCLE_TYPE
	{
		NV_GPU_CLIENT_ILLUM_PIECEWISE_LINEAR_CYCLE_HALF_HALT = 0,
		NV_GPU_CLIENT_ILLUM_PIECEWISE_LINEAR_CYCLE_FULL_HALT = 1,
		NV_GPU_CLIENT_ILLUM_PIECEWISE_LINEAR_CYCLE_FULL_REPEAT = 2,
		NV_GPU_CLIENT_ILLUM_PIECEWISE_LINEAR_CYCLE_INVALID = 255
	}

	public enum CLR_NV_SYSTEM_TYPE
	{
		NV_SYSTEM_TYPE_UNKNOWN,
		NV_SYSTEM_TYPE_LAPTOP,
		NV_SYSTEM_TYPE_DESKTOP
	}

	private bool isInitialized = false;

	private bool isGpuInDebugMode;

	private uint gpuCount;

	private unsafe NvPhysicalGpuHandle__*[] gpuHandles;

	private float[] powerTargetArray;

	private static volatile NvApi instance = null;

	private static object syncRoot = new object();

	private unsafe delegate* unmanaged[Cdecl, Cdecl]<NvPhysicalGpuHandle__*, NV_GPU_THERMAL_EX_V2*, _NvAPI_Status> NvAPI_GPU_GetAllTempsEx = null;

	public static NvApi Instance => Singleton_003CManagedNvApi_003A_003ANvApi_003E.Instance();

	public unsafe int MemoryClockOffsetMax
	{
		get
		{
			//IL_001b: Expected I, but got I8
			int num = 0;
			int num2 = 0;
			int result = 0;
			getClockOffset((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_PUBLIC_CLOCK_ID)4, &num, &num2, &result);
			return result;
		}
	}

	public unsafe int MemoryClockOffsetMin
	{
		get
		{
			//IL_001b: Expected I, but got I8
			int num = 0;
			int result = 0;
			int num2 = 0;
			getClockOffset((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_PUBLIC_CLOCK_ID)4, &num, &result, &num2);
			return result;
		}
	}

	public unsafe int MemoryClockOffset
	{
		get
		{
			//IL_001b: Expected I, but got I8
			int result = 0;
			int num = 0;
			int num2 = 0;
			getClockOffset((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_PUBLIC_CLOCK_ID)4, &result, &num, &num2);
			return result;
		}
		set
		{
			//IL_0010: Expected I, but got I8
			setClockOffset((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_PUBLIC_CLOCK_ID)4, value);
		}
	}

	public unsafe int GpuClockOffsetMax
	{
		get
		{
			//IL_001b: Expected I, but got I8
			int num = 0;
			int num2 = 0;
			int result = 0;
			getClockOffset((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_PUBLIC_CLOCK_ID)0, &num, &num2, &result);
			return result;
		}
	}

	public unsafe int GpuClockOffsetMin
	{
		get
		{
			//IL_001b: Expected I, but got I8
			int num = 0;
			int result = 0;
			int num2 = 0;
			getClockOffset((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_PUBLIC_CLOCK_ID)0, &num, &result, &num2);
			return result;
		}
	}

	public unsafe int GpuClockOffset
	{
		get
		{
			//IL_001b: Expected I, but got I8
			int result = 0;
			int num = 0;
			int num2 = 0;
			getClockOffset((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_PUBLIC_CLOCK_ID)0, &result, &num, &num2);
			return result;
		}
		set
		{
			//IL_0010: Expected I, but got I8
			setClockOffset((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_PUBLIC_CLOCK_ID)0, value);
		}
	}

	public unsafe float GpuTempTargetDefault
	{
		get
		{
			//IL_0014: Expected I4, but got I8
			//IL_002b: Expected I, but got I8
			float result = 0f;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
			*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
			if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) == (_NvAPI_Status)0 && (byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) != 0)
			{
				uint num = 0u;
				uint num2 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
				if (0 < num2)
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8)))) != 1)
						{
							num++;
							continue;
						}
						result = (float)(*(int*)((ref *(_003F*)((long)num * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 24))))) * 0.00390625f;
						break;
					}
					while (num < num2);
				}
			}
			return result;
		}
	}

	public unsafe float GpuTempTargetMax
	{
		get
		{
			//IL_0014: Expected I4, but got I8
			//IL_002b: Expected I, but got I8
			float result = 0f;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
			*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
			if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) == (_NvAPI_Status)0 && (byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) != 0)
			{
				uint num = 0u;
				uint num2 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
				if (0 < num2)
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8)))) != 1)
						{
							num++;
							continue;
						}
						result = (float)(*(int*)((ref *(_003F*)((long)num * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 28))))) * 0.00390625f;
						break;
					}
					while (num < num2);
				}
			}
			return result;
		}
	}

	public unsafe float GpuTempTargetMin
	{
		get
		{
			//IL_0014: Expected I4, but got I8
			//IL_002b: Expected I, but got I8
			float result = 0f;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
			*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
			if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) == (_NvAPI_Status)0 && (byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) != 0)
			{
				uint num = 0u;
				uint num2 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
				if (0 < num2)
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8)))) != 1)
						{
							num++;
							continue;
						}
						result = (float)(*(int*)((ref *(_003F*)((long)num * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 20))))) * 0.00390625f;
						break;
					}
					while (num < num2);
				}
			}
			return result;
		}
	}

	public unsafe float GpuTempTarget
	{
		get
		{
			//IL_0015: Expected I4, but got I8
			//IL_0023: Expected I4, but got I8
			//IL_003a: Expected I, but got I8
			//IL_00a4: Expected I, but got I8
			float result = 0f;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3 nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 0, 1352);
			*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
			if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) == (_NvAPI_Status)0 && (byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) != 0)
			{
				System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)) = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
				uint num = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
				if (0 < num)
				{
					long num2 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8));
					long num3 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8));
					uint num4 = num;
					do
					{
						*(int*)num2 = *(int*)num3;
						num3 += 348;
						num2 += 336;
						num4 += uint.MaxValue;
					}
					while (num4 != 0);
				}
				*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) = 197960;
				if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) == (_NvAPI_Status)0)
				{
					uint num5 = 0u;
					uint num6 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4));
					if (0 < num6)
					{
						do
						{
							if (*(int*)((ref *(_003F*)((long)num5 * 336L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8)))) != 1)
							{
								num5++;
								continue;
							}
							result = (float)(*(int*)((ref *(_003F*)((long)num5 * 336L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 16))))) * 0.00390625f;
							break;
						}
						while (num5 < num6);
					}
				}
			}
			return result;
		}
		set
		{
			//IL_000e: Expected I4, but got I8
			//IL_001c: Expected I4, but got I8
			//IL_0033: Expected I, but got I8
			//IL_00f9: Expected I, but got I8
			//IL_012b: Expected I, but got I8
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3 nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 0, 1352);
			*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
			if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) != 0 || (byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) == 0)
			{
				return;
			}
			uint num = 0u;
			if (0u >= (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5)))
			{
				return;
			}
			do
			{
				long num2 = (long)num * 348L;
				if (*(int*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8)))) == 1)
				{
					int num3 = (int)((double)value * 256.0);
					int num4 = *(int*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 28))));
					if (num3 > num4)
					{
						num3 = num4;
					}
					else
					{
						int num5 = *(int*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 20))));
						num3 = ((num3 < num5) ? num5 : num3);
					}
					*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) = 197960;
					System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)) = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
					uint num6 = 0u;
					if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5)))
					{
						do
						{
							long num7 = num6;
							*(int*)((ref *(_003F*)(num7 * 336)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8)))) = *(int*)((ref *(_003F*)(num7 * 348)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8))));
							num6++;
						}
						while (num6 < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)));
					}
					if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) == (_NvAPI_Status)0)
					{
						*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) = 197960;
						System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)) = 1;
						System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8)) = *(int*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8))));
						System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 16)) = num3;
						if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesSetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) == (_NvAPI_Status)0)
						{
							break;
						}
					}
				}
				num++;
			}
			while (num < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5)));
		}
	}

	public unsafe float PowerTargetDefault
	{
		get
		{
			//IL_0014: Expected I4, but got I8
			//IL_002b: Expected I, but got I8
			float result = 0f;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_POLICIES_INFO_V2 nV_GPU_CLIENT_POWER_POLICIES_INFO_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 0, 2248);
			*(int*)(&nV_GPU_CLIENT_POWER_POLICIES_INFO_V) = 133320;
			if (global::_003CModule_003E.NvAPI_GPU_ClientPowerPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_POWER_POLICIES_INFO_V) == (_NvAPI_Status)0 && System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, bool>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 4)))
			{
				uint num = 0u;
				uint num2 = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 5));
				if (0 < num2)
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num * 560L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 8)))) != 0)
						{
							num++;
							continue;
						}
						result = (float)(*(uint*)((ref *(_003F*)((long)num * 560L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 36))))) / 1000f;
						break;
					}
					while (num < num2);
				}
			}
			return result;
		}
	}

	public unsafe float PowerTargetMax
	{
		get
		{
			//IL_0014: Expected I4, but got I8
			//IL_002b: Expected I, but got I8
			float result = 0f;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_POLICIES_INFO_V2 nV_GPU_CLIENT_POWER_POLICIES_INFO_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 0, 2248);
			*(int*)(&nV_GPU_CLIENT_POWER_POLICIES_INFO_V) = 133320;
			if (global::_003CModule_003E.NvAPI_GPU_ClientPowerPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_POWER_POLICIES_INFO_V) == (_NvAPI_Status)0 && System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, bool>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 4)))
			{
				uint num = 0u;
				uint num2 = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 5));
				if (0 < num2)
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num * 560L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 8)))) != 0)
						{
							num++;
							continue;
						}
						result = (float)(*(uint*)((ref *(_003F*)((long)num * 560L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 48))))) / 1000f;
						break;
					}
					while (num < num2);
				}
			}
			return result;
		}
	}

	public unsafe float PowerTargetMin
	{
		get
		{
			//IL_0014: Expected I4, but got I8
			//IL_002b: Expected I, but got I8
			float result = 0f;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_POLICIES_INFO_V2 nV_GPU_CLIENT_POWER_POLICIES_INFO_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 0, 2248);
			*(int*)(&nV_GPU_CLIENT_POWER_POLICIES_INFO_V) = 133320;
			if (global::_003CModule_003E.NvAPI_GPU_ClientPowerPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_POWER_POLICIES_INFO_V) == (_NvAPI_Status)0 && System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, bool>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 4)))
			{
				uint num = 0u;
				uint num2 = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 5));
				if (0 < num2)
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num * 560L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 8)))) != 0)
						{
							num++;
							continue;
						}
						result = (float)(*(uint*)((ref *(_003F*)((long)num * 560L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 24))))) / 1000f;
						break;
					}
					while (num < num2);
				}
			}
			return result;
		}
	}

	public unsafe float PowerTarget
	{
		get
		{
			//IL_0014: Expected I4, but got I8
			//IL_002b: Expected I, but got I8
			float num = 0f;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_POLICIES_STATUS_V2 nV_GPU_CLIENT_POWER_POLICIES_STATUS_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_POLICIES_STATUS_V, 0, 1368);
			*(int*)(&nV_GPU_CLIENT_POWER_POLICIES_STATUS_V) = 132440;
			if (global::_003CModule_003E.NvAPI_GPU_ClientPowerPoliciesGetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_POWER_POLICIES_STATUS_V) == (_NvAPI_Status)0)
			{
				uint num2 = 0u;
				uint num3 = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_STATUS_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_STATUS_V, 4));
				if (0 < num3)
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num2 * 340L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_STATUS_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_STATUS_V, 8)))) != 0)
						{
							num2++;
							continue;
						}
						num = (float)(*(uint*)((ref *(_003F*)((long)num2 * 340L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_STATUS_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_STATUS_V, 20))))) / 1000f;
						break;
					}
					while (num2 < num3);
				}
			}
			powerTargetArray[nIndex] = num;
			return num;
		}
		set
		{
			//IL_000e: Expected I4, but got I8
			//IL_001c: Expected I4, but got I8
			//IL_0033: Expected I, but got I8
			//IL_00cf: Expected I, but got I8
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_POLICIES_INFO_V2 nV_GPU_CLIENT_POWER_POLICIES_INFO_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 0, 2248);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_POLICIES_STATUS_V2 nV_GPU_CLIENT_POWER_POLICIES_STATUS_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_POLICIES_STATUS_V, 0, 1368);
			*(int*)(&nV_GPU_CLIENT_POWER_POLICIES_INFO_V) = 133320;
			if (global::_003CModule_003E.NvAPI_GPU_ClientPowerPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_POWER_POLICIES_INFO_V) != 0 || System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 4)) == 0)
			{
				return;
			}
			uint num = 0u;
			if (0u >= (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 5)))
			{
				return;
			}
			do
			{
				long num2 = (long)num * 560L;
				if (*(int*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 8)))) == 0)
				{
					uint num3 = (uint)((double)value * 1000.0);
					uint num4 = *(uint*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 48))));
					if (num3 > num4)
					{
						num3 = num4;
					}
					else
					{
						uint num5 = *(uint*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 24))));
						num3 = ((num3 < num5) ? num5 : num3);
					}
					*(int*)(&nV_GPU_CLIENT_POWER_POLICIES_STATUS_V) = 132440;
					System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_STATUS_V2, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_STATUS_V, 4)) = 1;
					System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_STATUS_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_STATUS_V, 8)) = 0;
					System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_STATUS_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_STATUS_V, 16)) &= -2;
					System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_STATUS_V2, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_STATUS_V, 20)) = num3;
					if (global::_003CModule_003E.NvAPI_GPU_ClientPowerPoliciesSetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_POWER_POLICIES_STATUS_V) == (_NvAPI_Status)0)
					{
						break;
					}
				}
				num++;
			}
			while (num < System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 5)));
		}
	}

	public unsafe bool GpuFanSpeedAuto
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			//IL_0019: Expected I4, but got I8
			//IL_0031: Expected I, but got I8
			bool result = true;
			if (nIndex < gpuCount)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_GETCOOLER_SETTINGS_V4 nV_GPU_GETCOOLER_SETTINGS_V);
				// IL initblk instruction
				System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_GETCOOLER_SETTINGS_V, 0, 1368);
				*(int*)(&nV_GPU_GETCOOLER_SETTINGS_V) = 263512;
				if (global::_003CModule_003E.NvAPI_GPU_GetCoolerSettings((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], 0u, &nV_GPU_GETCOOLER_SETTINGS_V) == (_NvAPI_Status)0)
				{
					uint num = 0u;
					if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)))
					{
						do
						{
							if (*(int*)((ref *(_003F*)((long)num * 68L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 8)))) != 1)
							{
								num++;
								continue;
							}
							result = *(int*)((ref *(_003F*)((long)num * 68L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 40)))) != 1;
							break;
						}
						while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)));
					}
				}
			}
			return result;
		}
		[param: MarshalAs(UnmanagedType.U1)]
		set
		{
			//IL_000e: Expected I4, but got I8
			//IL_001c: Expected I4, but got I8
			//IL_0034: Expected I, but got I8
			//IL_0094: Expected I, but got I8
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_GETCOOLER_SETTINGS_V4 nV_GPU_GETCOOLER_SETTINGS_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_GETCOOLER_SETTINGS_V, 0, 1368);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_SETCOOLER_LEVEL_V2 nV_GPU_SETCOOLER_LEVEL_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_SETCOOLER_LEVEL_V, 0, 164);
			*(int*)(&nV_GPU_GETCOOLER_SETTINGS_V) = 263512;
			if (global::_003CModule_003E.NvAPI_GPU_GetCoolerSettings((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], 0u, &nV_GPU_GETCOOLER_SETTINGS_V) != 0)
			{
				return;
			}
			uint num = 0u;
			if (0u >= (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)))
			{
				return;
			}
			do
			{
				long num2 = num;
				long num3 = num2 * 68;
				if (*(int*)((ref *(_003F*)num3) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 8)))) == 1)
				{
					*(int*)(&nV_GPU_SETCOOLER_LEVEL_V) = 65700;
					long num4 = num2 * 8;
					*(int*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_SETCOOLER_LEVEL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_SETCOOLER_LEVEL_V, 4)))) = *(int*)((ref *(_003F*)num3) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 32))));
					NV_COOLER_POLICY nV_COOLER_POLICY = ((!value) ? ((NV_COOLER_POLICY)1) : ((NV_COOLER_POLICY)32));
					*(NV_COOLER_POLICY*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_SETCOOLER_LEVEL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_SETCOOLER_LEVEL_V, 8)))) = nV_COOLER_POLICY;
					if (global::_003CModule_003E.NvAPI_GPU_SetCoolerLevels((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], num, &nV_GPU_SETCOOLER_LEVEL_V) == (_NvAPI_Status)0)
					{
						break;
					}
				}
				num++;
			}
			while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)));
		}
	}

	public unsafe uint GpuFanSpeedMax
	{
		get
		{
			//IL_000e: Expected I4, but got I8
			//IL_0028: Expected I, but got I8
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_GETCOOLER_SETTINGS_V4 nV_GPU_GETCOOLER_SETTINGS_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_GETCOOLER_SETTINGS_V, 0, 1368);
			uint result = 0u;
			*(int*)(&nV_GPU_GETCOOLER_SETTINGS_V) = 263512;
			if (global::_003CModule_003E.NvAPI_GPU_GetCoolerSettings((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], 0u, &nV_GPU_GETCOOLER_SETTINGS_V) == (_NvAPI_Status)0)
			{
				uint num = 0u;
				if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)))
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num * 68L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 8)))) != 1)
						{
							num++;
							continue;
						}
						result = *(uint*)((ref *(_003F*)((long)num * 68L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 28))));
						break;
					}
					while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)));
				}
			}
			return result;
		}
	}

	public unsafe uint GpuFanSpeedMin
	{
		get
		{
			//IL_000e: Expected I4, but got I8
			//IL_0028: Expected I, but got I8
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_GETCOOLER_SETTINGS_V4 nV_GPU_GETCOOLER_SETTINGS_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_GETCOOLER_SETTINGS_V, 0, 1368);
			uint result = 0u;
			*(int*)(&nV_GPU_GETCOOLER_SETTINGS_V) = 263512;
			if (global::_003CModule_003E.NvAPI_GPU_GetCoolerSettings((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], 0u, &nV_GPU_GETCOOLER_SETTINGS_V) == (_NvAPI_Status)0)
			{
				uint num = 0u;
				if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)))
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num * 68L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 8)))) != 1)
						{
							num++;
							continue;
						}
						result = *(uint*)((ref *(_003F*)((long)num * 68L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 24))));
						break;
					}
					while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)));
				}
			}
			return result;
		}
	}

	public unsafe uint GpuFanSpeed
	{
		get
		{
			//IL_000e: Expected I4, but got I8
			//IL_0028: Expected I, but got I8
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_GETCOOLER_SETTINGS_V4 nV_GPU_GETCOOLER_SETTINGS_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_GETCOOLER_SETTINGS_V, 0, 1368);
			uint result = 0u;
			*(int*)(&nV_GPU_GETCOOLER_SETTINGS_V) = 263512;
			if (global::_003CModule_003E.NvAPI_GPU_GetCoolerSettings((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], 0u, &nV_GPU_GETCOOLER_SETTINGS_V) == (_NvAPI_Status)0)
			{
				uint num = 0u;
				if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)))
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num * 68L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 8)))) != 1)
						{
							num++;
							continue;
						}
						result = *(uint*)((ref *(_003F*)((long)num * 68L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 32))));
						break;
					}
					while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)));
				}
			}
			return result;
		}
		set
		{
			//IL_000e: Expected I4, but got I8
			//IL_001c: Expected I4, but got I8
			//IL_0034: Expected I, but got I8
			//IL_00b8: Expected I, but got I8
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_GETCOOLER_SETTINGS_V4 nV_GPU_GETCOOLER_SETTINGS_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_GETCOOLER_SETTINGS_V, 0, 1368);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_SETCOOLER_LEVEL_V2 nV_GPU_SETCOOLER_LEVEL_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_SETCOOLER_LEVEL_V, 0, 164);
			*(int*)(&nV_GPU_GETCOOLER_SETTINGS_V) = 263512;
			if (global::_003CModule_003E.NvAPI_GPU_GetCoolerSettings((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], 0u, &nV_GPU_GETCOOLER_SETTINGS_V) != 0)
			{
				return;
			}
			uint num = 0u;
			if (0u >= (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)))
			{
				return;
			}
			do
			{
				long num2 = num;
				long num3 = num2 * 68;
				if (*(int*)((ref *(_003F*)num3) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 8)))) == 1)
				{
					uint num4 = *(uint*)((ref *(_003F*)num3) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 24))));
					if (value < num4)
					{
						value = num4;
					}
					else
					{
						uint num5 = *(uint*)((ref *(_003F*)num3) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 28))));
						value = ((value > num5) ? num5 : value);
					}
					*(int*)(&nV_GPU_SETCOOLER_LEVEL_V) = 65700;
					long num6 = num2 * 8;
					*(int*)((ref *(_003F*)num6) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_SETCOOLER_LEVEL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_SETCOOLER_LEVEL_V, 8)))) = *(int*)((ref *(_003F*)num3) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 40))));
					*(uint*)((ref *(_003F*)num6) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_SETCOOLER_LEVEL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_SETCOOLER_LEVEL_V, 4)))) = value;
					if (global::_003CModule_003E.NvAPI_GPU_SetCoolerLevels((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], num, &nV_GPU_SETCOOLER_LEVEL_V) == (_NvAPI_Status)0)
					{
						break;
					}
				}
				num++;
			}
			while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)));
		}
	}

	public unsafe uint GpuMemoryClock
	{
		get
		{
			//IL_001d: Expected I, but got I8
			uint result = 0u;
			if (nIndex < gpuCount)
			{
				getClocks((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE)0, (_NV_GPU_PUBLIC_CLOCK_ID)4, &result);
			}
			return result;
		}
	}

	public unsafe uint GpuGraphicsClock
	{
		get
		{
			//IL_001d: Expected I, but got I8
			uint result = 0u;
			if (nIndex < gpuCount)
			{
				getClocks((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE)0, (_NV_GPU_PUBLIC_CLOCK_ID)0, &result);
			}
			return result;
		}
	}

	public unsafe float GpuTemperature
	{
		get
		{
			//IL_0011: Expected I4, but got I8
			//IL_0029: Expected I, but got I8
			float result = 0f;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_THERMAL_SETTINGS_V2 nV_GPU_THERMAL_SETTINGS_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_THERMAL_SETTINGS_V, 0, 68);
			*(int*)(&nV_GPU_THERMAL_SETTINGS_V) = 131140;
			if (global::_003CModule_003E.NvAPI_GPU_GetThermalSettings((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], 0u, &nV_GPU_THERMAL_SETTINGS_V) == (_NvAPI_Status)0)
			{
				uint num = 0u;
				if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_THERMAL_SETTINGS_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_THERMAL_SETTINGS_V, 4)))
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num * 20L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_THERMAL_SETTINGS_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_THERMAL_SETTINGS_V, 24)))) != 1)
						{
							num++;
							continue;
						}
						result = *(int*)((ref *(_003F*)((long)num * 20L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_THERMAL_SETTINGS_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_THERMAL_SETTINGS_V, 20))));
						break;
					}
					while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_THERMAL_SETTINGS_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_THERMAL_SETTINGS_V, 4)));
				}
			}
			return result;
		}
	}

	public unsafe string GpuGetFullName
	{
		get
		{
			//IL_0029: Expected I4, but got I8
			//IL_0038: Expected I, but got I8
			string result = "";
			if (nIndex < gpuCount)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0EA_0040D _0024ArrayType_0024_0024_0024BY0EA_0040D);
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref _0024ArrayType_0024_0024_0024BY0EA_0040D, ref global::_003CModule_003E._003F_003F_C_0040_00CNPNBAHC_0040_0040, 1);
				// IL initblk instruction
				System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY0EA_0040D, 1), 0, 63);
				if (global::_003CModule_003E.NvAPI_GPU_GetFullName((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (sbyte*)(&_0024ArrayType_0024_0024_0024BY0EA_0040D)) == (_NvAPI_Status)0)
				{
					result = new string((sbyte*)(&_0024ArrayType_0024_0024_0024BY0EA_0040D));
				}
			}
			return result;
		}
	}

	public uint GpuCount => gpuCount;

	public bool IsGpuInDebugMode
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return isGpuInDebugMode;
		}
	}

	public bool IsInitialized
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return isInitialized;
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getNormalRead(uint nIndex, byte adr, byte reg, byte[] datas, int portId)
	{
		//IL_0023: Expected I4, but got I8
		//IL_0093: Expected I, but got I8
		//IL_00af: Expected I, but got I8
		int num = datas.Length;
		if (num >= 384)
		{
			return false;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_I2C_INFO_EX nV_I2C_INFO_EX);
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_EX, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_EX, 4)) = 0;
		*(int*)(&nV_I2C_INFO_EX) = 0;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_I2C_INFO_V3 nV_I2C_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_I2C_INFO_V, 0, 64);
		*(int*)(&nV_I2C_INFO_V) = 196672;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 8)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 4)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 44)) = 65535;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 48)) = 0;
		if (portId != 0)
		{
			System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 56)) = 1;
			System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 52)) = (byte)portId;
		}
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 9)) = adr;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 16)) = (nint)(&reg);
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 24)) = 1;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0BIA_0040E _0024ArrayType_0024_0024_0024BY0BIA_0040E);
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 32)) = (nint)(&_0024ArrayType_0024_0024_0024BY0BIA_0040E);
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 40)) = num;
		if (global::_003CModule_003E.NvAPI_I2CReadEx((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_I2C_INFO_V, &nV_I2C_INFO_EX) == (_NvAPI_Status)0)
		{
			int num2 = 0;
			if (0 < (nint)datas.LongLength)
			{
				_0024ArrayType_0024_0024_0024BY0BIA_0040E* ptr = &_0024ArrayType_0024_0024_0024BY0BIA_0040E;
				do
				{
					datas[num2] = *(byte*)ptr;
					num2++;
					ptr = (_0024ArrayType_0024_0024_0024BY0BIA_0040E*)((ulong)(nint)ptr + 1uL);
				}
				while (num2 < (nint)datas.LongLength);
			}
			return true;
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool setNormalWrite(uint nIndex, byte adr, byte reg, byte[] datas, int portId)
	{
		//IL_0041: Expected I4, but got I8
		//IL_00b1: Expected I, but got I8
		//IL_0028: Expected I, but got I8
		int num = datas.Length;
		if (num >= 384)
		{
			return false;
		}
		int num2 = 0;
		int num3 = num;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0BIA_0040E _0024ArrayType_0024_0024_0024BY0BIA_0040E);
		if (0 < num3)
		{
			_0024ArrayType_0024_0024_0024BY0BIA_0040E* ptr = &_0024ArrayType_0024_0024_0024BY0BIA_0040E;
			do
			{
				*(byte*)ptr = datas[num2];
				num2++;
				ptr = (_0024ArrayType_0024_0024_0024BY0BIA_0040E*)((ulong)(nint)ptr + 1uL);
			}
			while (num2 < num3);
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_I2C_INFO_EX nV_I2C_INFO_EX);
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_EX, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_EX, 4)) = 0;
		*(int*)(&nV_I2C_INFO_EX) = 0;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_I2C_INFO_V3 nV_I2C_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_I2C_INFO_V, 0, 64);
		*(int*)(&nV_I2C_INFO_V) = 196672;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 8)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 4)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 44)) = 65535;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 48)) = 0;
		if (portId != 0)
		{
			System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 56)) = 1;
			System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 52)) = (byte)portId;
		}
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 9)) = adr;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 16)) = (nint)(&reg);
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 24)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 32)) = (nint)(&_0024ArrayType_0024_0024_0024BY0BIA_0040E);
		System.Runtime.CompilerServices.Unsafe.As<NV_I2C_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_I2C_INFO_V, 40)) = num;
		return global::_003CModule_003E.NvAPI_I2CWriteEx((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_I2C_INFO_V, &nV_I2C_INFO_EX) == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getSysDriverAndBranchVersion(ref uint dwDriverVersion, ref string strBuildBranchString)
	{
		//IL_001c: Expected I4, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0EA_0040D _0024ArrayType_0024_0024_0024BY0EA_0040D);
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref _0024ArrayType_0024_0024_0024BY0EA_0040D, ref global::_003CModule_003E._003F_003F_C_0040_00CNPNBAHC_0040_0040, 1);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY0EA_0040D, 1), 0, 63);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (global::_003CModule_003E.NvAPI_SYS_GetDriverAndBranchVersion(&num, (sbyte*)(&_0024ArrayType_0024_0024_0024BY0EA_0040D)) == (_NvAPI_Status)0)
		{
			dwDriverVersion = num;
			strBuildBranchString = new string((sbyte*)(&_0024ArrayType_0024_0024_0024BY0EA_0040D));
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool getGpuGraphicsClockRange(uint nIndex, ref uint ulMinFrequency, ref uint ulMaxFrequency)
	{
		ulMinFrequency = 0u;
		ulMaxFrequency = 2500000u;
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuProcessorClock(uint nIndex, ref uint ulFrequency)
	{
		//IL_0014: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getClocks((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE)0, (_NV_GPU_PUBLIC_CLOCK_ID)7, &num))
		{
			ulFrequency = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool getGpuProcessorClockRange(uint nIndex, ref uint ulMinFrequency, ref uint ulMaxFrequency)
	{
		ulMinFrequency = 0u;
		ulMaxFrequency = 3000000u;
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool getGpuMemoryClockRange(uint nIndex, ref uint ulMinFrequency, ref uint ulMaxFrequency)
	{
		ulMinFrequency = 0u;
		ulMaxFrequency = 11000000u;
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuBaseGraphicsClock(uint nIndex, ref uint ulFrequency)
	{
		//IL_0014: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getClocks((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE)1, (_NV_GPU_PUBLIC_CLOCK_ID)0, &num))
		{
			ulFrequency = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuBoostGraphicsClock(uint nIndex, ref uint ulFrequency)
	{
		//IL_0014: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getClocks((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE)2, (_NV_GPU_PUBLIC_CLOCK_ID)0, &num))
		{
			ulFrequency = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuBaseMemoryClock(uint nIndex, ref uint ulFrequency)
	{
		//IL_0014: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getClocks((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE)1, (_NV_GPU_PUBLIC_CLOCK_ID)4, &num))
		{
			ulFrequency = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuBoostMemoryClock(uint nIndex, ref uint ulFrequency)
	{
		//IL_0014: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getClocks((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE)2, (_NV_GPU_PUBLIC_CLOCK_ID)4, &num))
		{
			ulFrequency = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool getGpuFanTachometer(uint nIndex, uint nFanIndex, ref uint ulFanTachometer)
	{
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint nRpmCurr);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint nMin);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint nMax);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint nTarget);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint nCoolers);
		if (getFanCoolerStatus(nIndex, nFanIndex, ref nRpmCurr, ref nMin, ref nMax, ref nTarget, ref nCoolers))
		{
			ulFanTachometer = nRpmCurr;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuFanTachometer(uint nIndex, ref uint ulFanTachometer)
	{
		//IL_0010: Expected I4, but got I8
		//IL_0028: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_GETCOOLER_SETTINGS_V4 nV_GPU_GETCOOLER_SETTINGS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_GETCOOLER_SETTINGS_V, 0, 1368);
		*(int*)(&nV_GPU_GETCOOLER_SETTINGS_V) = 263512;
		if (global::_003CModule_003E.NvAPI_GPU_GetCoolerSettings((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], 0u, &nV_GPU_GETCOOLER_SETTINGS_V) == (_NvAPI_Status)0)
		{
			uint num = 0u;
			if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)))
			{
				do
				{
					long num2 = (long)num * 68L;
					if (*(int*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 8)))) != 1 || *(byte*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 60)))) == 0)
					{
						num++;
						continue;
					}
					ulFanTachometer = *(uint*)((ref *(_003F*)((long)num * 68L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 56))));
					result = true;
					break;
				}
				while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)));
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuFanTachometerRange(uint nIndex, uint nFanIndex, ref uint ulMinFanTachometer, ref uint ulMaxFanTachometer)
	{
		//IL_0020: Expected I, but got I8
		bool result = false;
		if (nIndex >= gpuCount)
		{
			return false;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
		if (getFanCoolerInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], nFanIndex, &num, &num2))
		{
			ulMinFanTachometer = num;
			ulMaxFanTachometer = num2;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuFanTachometerRange(uint nIndex, ref uint ulMinFanTachometer, ref uint ulMaxFanTachometer)
	{
		//IL_0010: Expected I4, but got I8
		//IL_0028: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_GETCOOLER_SETTINGS_V4 nV_GPU_GETCOOLER_SETTINGS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_GETCOOLER_SETTINGS_V, 0, 1368);
		*(int*)(&nV_GPU_GETCOOLER_SETTINGS_V) = 263512;
		if (global::_003CModule_003E.NvAPI_GPU_GetCoolerSettings((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], 0u, &nV_GPU_GETCOOLER_SETTINGS_V) == (_NvAPI_Status)0)
		{
			uint num = 0u;
			if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)))
			{
				do
				{
					long num2 = (long)num * 68L;
					if (*(int*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 8)))) != 1 || *(byte*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 60)))) == 0)
					{
						num++;
						continue;
					}
					ulMinFanTachometer = 0u;
					ulMaxFanTachometer = 5000u;
					result = true;
					break;
				}
				while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_GETCOOLER_SETTINGS_V4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_GETCOOLER_SETTINGS_V, 4)));
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuPower(uint nIndex, ref uint ulPower, CHANNEL_ID enChannelId)
	{
		//IL_001f: Expected I4, but got I8
		//IL_002a: Expected I4, but got I8
		//IL_0041: Expected I, but got I8
		//IL_009f: Expected I, but got I8
		bool result = false;
		NV_GPU_CLIENT_POWER_TOPOLOGY_CHANNEL_ID nV_GPU_CLIENT_POWER_TOPOLOGY_CHANNEL_ID;
		switch (enChannelId)
		{
		default:
			return false;
		case CHANNEL_ID.CHANNEL_ID_NORMALIZED_TOTAL_POWER:
			nV_GPU_CLIENT_POWER_TOPOLOGY_CHANNEL_ID = (NV_GPU_CLIENT_POWER_TOPOLOGY_CHANNEL_ID)1;
			break;
		case CHANNEL_ID.CHANNEL_ID_TOTAL_GPU_POWER:
			nV_GPU_CLIENT_POWER_TOPOLOGY_CHANNEL_ID = (NV_GPU_CLIENT_POWER_TOPOLOGY_CHANNEL_ID)0;
			break;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V1 nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V, 0, 24);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1 nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 0, 72);
		*(int*)(&nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V) = 65560;
		if (global::_003CModule_003E.NvAPI_GPU_ClientPowerTopologyGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V) == (_NvAPI_Status)0 && System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V1, bool>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V, 4)))
		{
			*(int*)(&nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V) = 65608;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 4)) = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V, 5));
			uint num = 0u;
			if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V, 5)))
			{
				do
				{
					long num2 = num;
					*(int*)((ref *(_003F*)(num2 * 16)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 8)))) = *(int*)((ref *(_003F*)(num2 * 4)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V, 8))));
					num++;
				}
				while (num < System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 4)));
			}
			if (global::_003CModule_003E.NvAPI_GPU_ClientPowerTopologyGetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V) == (_NvAPI_Status)0)
			{
				uint num3 = 0u;
				uint num4 = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 4));
				if (0 < num4)
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num3 * 16L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 8)))) != (int)nV_GPU_CLIENT_POWER_TOPOLOGY_CHANNEL_ID)
						{
							num3++;
							continue;
						}
						ulPower = (uint)(*(int*)((ref *(_003F*)((long)num3 * 16L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 16))))) / 1000u;
						result = true;
						break;
					}
					while (num3 < num4);
				}
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuPowerRange(uint nIndex, ref uint ulMinPower, ref uint ulMaxPower)
	{
		//IL_000e: Expected I4, but got I8
		//IL_0019: Expected I4, but got I8
		//IL_0030: Expected I, but got I8
		//IL_008e: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V1 nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V, 0, 24);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1 nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 0, 72);
		*(int*)(&nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V) = 65560;
		if (global::_003CModule_003E.NvAPI_GPU_ClientPowerTopologyGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V) == (_NvAPI_Status)0 && System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V1, bool>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V, 4)))
		{
			*(int*)(&nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V) = 65608;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 4)) = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V, 5));
			uint num = 0u;
			if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V, 5)))
			{
				do
				{
					long num2 = num;
					*(int*)((ref *(_003F*)(num2 * 16)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 8)))) = *(int*)((ref *(_003F*)(num2 * 4)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_INFO_V, 8))));
					num++;
				}
				while (num < System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 4)));
			}
			if (global::_003CModule_003E.NvAPI_GPU_ClientPowerTopologyGetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V) == (_NvAPI_Status)0)
			{
				uint num3 = 0u;
				uint num4 = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 4));
				if (0 < num4)
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num3 * 16L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_TOPOLOGY_STATUS_V, 8)))) != 1)
						{
							num3++;
							continue;
						}
						ulMinPower = 0u;
						ulMaxPower = 150u;
						result = true;
						break;
					}
					while (num3 < num4);
				}
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuUsage(uint nIndex, ref uint ulPercentage)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getUsages((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_UTILIZATION_DOMAIN_ID)0, &num))
		{
			ulPercentage = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuUsageRange(uint nIndex, ref uint ulMinPercentage, ref uint ulMaxPercentage)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getUsages((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_UTILIZATION_DOMAIN_ID)0, &num))
		{
			ulMinPercentage = 0u;
			ulMaxPercentage = 100u;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getFrameBufferUsage(uint nIndex, ref uint ulPercentage)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getUsages((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_UTILIZATION_DOMAIN_ID)1, &num))
		{
			ulPercentage = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getFrameBufferUsageRange(uint nIndex, ref uint ulMinPercentage, ref uint ulMaxPercentage)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getUsages((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_UTILIZATION_DOMAIN_ID)1, &num))
		{
			ulMinPercentage = 0u;
			ulMaxPercentage = 100u;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getVideoUsage(uint nIndex, ref uint ulPercentage)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getUsages((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_UTILIZATION_DOMAIN_ID)2, &num))
		{
			ulPercentage = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getVideoUsageRange(uint nIndex, ref uint ulMinPercentage, ref uint ulMaxPercentage)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getUsages((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_UTILIZATION_DOMAIN_ID)2, &num))
		{
			ulMinPercentage = 0u;
			ulMaxPercentage = 100u;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getBusUsage(uint nIndex, ref uint ulPercentage)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getUsages((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_UTILIZATION_DOMAIN_ID)3, &num))
		{
			ulPercentage = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getBusUsageRange(uint nIndex, ref uint ulMinPercentage, ref uint ulMaxPercentage)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getUsages((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (_NV_GPU_UTILIZATION_DOMAIN_ID)3, &num))
		{
			ulMinPercentage = 0u;
			ulMaxPercentage = 100u;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getMemoryUsage(uint nIndex, ref uint ulMemory)
	{
		//IL_000d: Expected I4, but got I8
		//IL_0024: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_DISPLAY_DRIVER_MEMORY_INFO_V3 nV_DISPLAY_DRIVER_MEMORY_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_DISPLAY_DRIVER_MEMORY_INFO_V, 0, 32);
		*(int*)(&nV_DISPLAY_DRIVER_MEMORY_INFO_V) = 196640;
		if (global::_003CModule_003E.NvAPI_GPU_GetMemoryInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_DISPLAY_DRIVER_MEMORY_INFO_V) == (_NvAPI_Status)0)
		{
			ulMemory = (uint)(System.Runtime.CompilerServices.Unsafe.As<NV_DISPLAY_DRIVER_MEMORY_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_DISPLAY_DRIVER_MEMORY_INFO_V, 4)) - System.Runtime.CompilerServices.Unsafe.As<NV_DISPLAY_DRIVER_MEMORY_INFO_V3, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_DISPLAY_DRIVER_MEMORY_INFO_V, 20)));
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getMemoryUsageRange(uint nIndex, ref uint ulMinMemory, ref uint ulMaxMemory)
	{
		//IL_000d: Expected I4, but got I8
		//IL_0024: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_DISPLAY_DRIVER_MEMORY_INFO_V3 nV_DISPLAY_DRIVER_MEMORY_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_DISPLAY_DRIVER_MEMORY_INFO_V, 0, 32);
		*(int*)(&nV_DISPLAY_DRIVER_MEMORY_INFO_V) = 196640;
		if (global::_003CModule_003E.NvAPI_GPU_GetMemoryInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_DISPLAY_DRIVER_MEMORY_INFO_V) == (_NvAPI_Status)0)
		{
			ulMinMemory = 0u;
			ulMaxMemory = System.Runtime.CompilerServices.Unsafe.As<NV_DISPLAY_DRIVER_MEMORY_INFO_V3, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_DISPLAY_DRIVER_MEMORY_INFO_V, 4));
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuPowerLimit(uint nIndex, ref uint ulLimit)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getPerfPolicyStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_PERF_POLICY_ID_SW)0, &num))
		{
			ulLimit = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuPowerLimitRange(uint nIndex, ref uint ulMinLimit, ref uint ulMaxLimit)
	{
		//IL_0015: Expected I4, but got I8
		//IL_0024: Expected I, but got I8
		bool result = false;
		long num = ((long[])(object)gpuHandles)[nIndex];
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_POLICIES_INFO_PARAMS_V1 nV_GPU_PERF_POLICIES_INFO_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 0, 76);
		*(int*)(&nV_GPU_PERF_POLICIES_INFO_PARAMS_V) = 65612;
		if (global::_003CModule_003E.NvAPI_GPU_PerfPoliciesGetInfo((NvPhysicalGpuHandle__*)num, &nV_GPU_PERF_POLICIES_INFO_PARAMS_V) == (_NvAPI_Status)0 && (System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_POLICIES_INFO_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 8)) & 1) != 0)
		{
			ulMinLimit = 0u;
			ulMaxLimit = 1u;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuTempLimit(uint nIndex, ref uint ulLimit)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getPerfPolicyStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_PERF_POLICY_ID_SW)1, &num))
		{
			ulLimit = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuTempLimitRange(uint nIndex, ref uint ulMinLimit, ref uint ulMaxLimit)
	{
		//IL_0015: Expected I4, but got I8
		//IL_0024: Expected I, but got I8
		bool result = false;
		long num = ((long[])(object)gpuHandles)[nIndex];
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_POLICIES_INFO_PARAMS_V1 nV_GPU_PERF_POLICIES_INFO_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 0, 76);
		*(int*)(&nV_GPU_PERF_POLICIES_INFO_PARAMS_V) = 65612;
		if (global::_003CModule_003E.NvAPI_GPU_PerfPoliciesGetInfo((NvPhysicalGpuHandle__*)num, &nV_GPU_PERF_POLICIES_INFO_PARAMS_V) == (_NvAPI_Status)0 && (System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_POLICIES_INFO_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 8)) & 2) != 0)
		{
			ulMinLimit = 0u;
			ulMaxLimit = 1u;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuVoltageLimit(uint nIndex, ref uint ulLimit)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getPerfPolicyStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_PERF_POLICY_ID_SW)2, &num))
		{
			ulLimit = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuVoltageLimitRange(uint nIndex, ref uint ulMinLimit, ref uint ulMaxLimit)
	{
		//IL_0015: Expected I4, but got I8
		//IL_0024: Expected I, but got I8
		bool result = false;
		long num = ((long[])(object)gpuHandles)[nIndex];
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_POLICIES_INFO_PARAMS_V1 nV_GPU_PERF_POLICIES_INFO_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 0, 76);
		*(int*)(&nV_GPU_PERF_POLICIES_INFO_PARAMS_V) = 65612;
		if (global::_003CModule_003E.NvAPI_GPU_PerfPoliciesGetInfo((NvPhysicalGpuHandle__*)num, &nV_GPU_PERF_POLICIES_INFO_PARAMS_V) == (_NvAPI_Status)0 && (System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_POLICIES_INFO_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 8)) & 4) != 0)
		{
			ulMinLimit = 0u;
			ulMaxLimit = 1u;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuOperatingVoltageMaxLimit(uint nIndex, ref uint ulLimit)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getPerfPolicyStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_PERF_POLICY_ID_SW)3, &num))
		{
			ulLimit = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuOperatingVoltageMaxLimitRange(uint nIndex, ref uint ulMinLimit, ref uint ulMaxLimit)
	{
		//IL_0015: Expected I4, but got I8
		//IL_0024: Expected I, but got I8
		bool result = false;
		long num = ((long[])(object)gpuHandles)[nIndex];
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_POLICIES_INFO_PARAMS_V1 nV_GPU_PERF_POLICIES_INFO_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 0, 76);
		*(int*)(&nV_GPU_PERF_POLICIES_INFO_PARAMS_V) = 65612;
		if (global::_003CModule_003E.NvAPI_GPU_PerfPoliciesGetInfo((NvPhysicalGpuHandle__*)num, &nV_GPU_PERF_POLICIES_INFO_PARAMS_V) == (_NvAPI_Status)0 && (System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_POLICIES_INFO_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 8)) & 8) != 0)
		{
			ulMinLimit = 0u;
			ulMaxLimit = 1u;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuUtilizationLimit(uint nIndex, ref uint ulLimit)
	{
		//IL_0013: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (getPerfPolicyStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_PERF_POLICY_ID_SW)4, &num))
		{
			ulLimit = num;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuUtilizationLimitRange(uint nIndex, ref uint ulMinLimit, ref uint ulMaxLimit)
	{
		//IL_0015: Expected I4, but got I8
		//IL_0024: Expected I, but got I8
		bool result = false;
		long num = ((long[])(object)gpuHandles)[nIndex];
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_POLICIES_INFO_PARAMS_V1 nV_GPU_PERF_POLICIES_INFO_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 0, 76);
		*(int*)(&nV_GPU_PERF_POLICIES_INFO_PARAMS_V) = 65612;
		if (global::_003CModule_003E.NvAPI_GPU_PerfPoliciesGetInfo((NvPhysicalGpuHandle__*)num, &nV_GPU_PERF_POLICIES_INFO_PARAMS_V) == (_NvAPI_Status)0 && (System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_POLICIES_INFO_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 8)) & 0x10) != 0)
		{
			ulMinLimit = 0u;
			ulMaxLimit = 1u;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getPCIIdentifiers(uint nIndex, ref uint pDeviceId, ref uint pSubSystemId, ref uint pRevisionId, ref uint pExtDeviceId)
	{
		//IL_0017: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num4);
		if (global::_003CModule_003E.NvAPI_GPU_GetPCIIdentifiers((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &num, &num2, &num3, &num4) == (_NvAPI_Status)0)
		{
			pDeviceId = num;
			pSubSystemId = num2;
			pRevisionId = num3;
			pExtDeviceId = num4;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getGpuVbiosVersionString(uint nIndex, ref string strBiosRevision)
	{
		//IL_001c: Expected I4, but got I8
		//IL_002b: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0EA_0040D _0024ArrayType_0024_0024_0024BY0EA_0040D);
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref _0024ArrayType_0024_0024_0024BY0EA_0040D, ref global::_003CModule_003E._003F_003F_C_0040_00CNPNBAHC_0040_0040, 1);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY0EA_0040D, 1), 0, 63);
		if (global::_003CModule_003E.NvAPI_GPU_GetVbiosVersionString((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (sbyte*)(&_0024ArrayType_0024_0024_0024BY0EA_0040D)) == (_NvAPI_Status)0)
		{
			strBiosRevision = new string((sbyte*)(&_0024ArrayType_0024_0024_0024BY0EA_0040D));
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getClientVolRailStatus(uint nIndex, ref uint vol)
	{
		//IL_000d: Expected I4, but got I8
		//IL_0024: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_VOLT_RAILS_STATUS_V1 nV_GPU_CLIENT_VOLT_RAILS_STATUS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_VOLT_RAILS_STATUS_V, 0, 76);
		*(int*)(&nV_GPU_CLIENT_VOLT_RAILS_STATUS_V) = 65612;
		if (global::_003CModule_003E.NvAPI_GPU_ClientVoltRailsGetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_VOLT_RAILS_STATUS_V) == (_NvAPI_Status)0)
		{
			int num = 0;
			long num2 = 0L;
			long num3 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_VOLT_RAILS_STATUS_V, 36));
			do
			{
				if (*(int*)num3 != 0)
				{
					num++;
					num2++;
					num3 += 40;
					continue;
				}
				vol = *(uint*)((ref *(_003F*)((long)num * 40L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_VOLT_RAILS_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_VOLT_RAILS_STATUS_V, 40))));
				result = true;
				break;
			}
			while (num2 < 1);
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getClientVoltRailsControl(uint nIndex, ref uint uiDelta)
	{
		//IL_000b: Expected I4, but got I8
		//IL_0022: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_VOLT_RAILS_CONTROL_V1 nV_GPU_CLIENT_VOLT_RAILS_CONTROL_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_VOLT_RAILS_CONTROL_V, 0, 40);
		*(int*)(&nV_GPU_CLIENT_VOLT_RAILS_CONTROL_V) = 65576;
		if (global::_003CModule_003E.NvAPI_GPU_ClientVoltRailsGetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_VOLT_RAILS_CONTROL_V) != 0)
		{
			return false;
		}
		uiDelta = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_VOLT_RAILS_CONTROL_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_VOLT_RAILS_CONTROL_V, 4));
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool setClientVoltRailsControl(uint nIndex, uint uiDelta)
	{
		//IL_000b: Expected I4, but got I8
		//IL_002e: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_VOLT_RAILS_CONTROL_V1 nV_GPU_CLIENT_VOLT_RAILS_CONTROL_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_VOLT_RAILS_CONTROL_V, 0, 40);
		if (uiDelta <= 100)
		{
			*(int*)(&nV_GPU_CLIENT_VOLT_RAILS_CONTROL_V) = 65576;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_VOLT_RAILS_CONTROL_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_VOLT_RAILS_CONTROL_V, 4)) = (byte)uiDelta;
			return global::_003CModule_003E.NvAPI_GPU_ClientVoltRailsSetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_VOLT_RAILS_CONTROL_V) == (_NvAPI_Status)0;
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getThermalInfo(uint nIndex, ref int iT0X, ref uint uiT0Y, ref int iT1X, ref uint uiT1Y, ref uint uiT1OCY, ref int iT2X, ref uint uiT2Y, ref uint uiT2OCY, ref int iT3X, ref uint uiT3Y, ref uint uiT3OCY, ref int iT4X)
	{
		//IL_0011: Expected I4, but got I8
		//IL_001f: Expected I4, but got I8
		//IL_0036: Expected I, but got I8
		//IL_00ce: Expected I, but got I8
		//IL_0111: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3 nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 0, 1352);
		*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
		if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) != 0)
		{
			return false;
		}
		if ((byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) == 0)
		{
			return false;
		}
		uint num = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
		if (0 < num)
		{
			long num2 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 28));
			uint num3 = num;
			int num4;
			do
			{
				num4 = *(int*)num2;
				num2 += 348;
				num3 += uint.MaxValue;
			}
			while (num3 != 0);
			iT4X = num4;
		}
		*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) = 197960;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)) = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
		uint num5 = 0u;
		if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5)))
		{
			do
			{
				long num6 = num5;
				*(int*)((ref *(_003F*)(num6 * 336)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8)))) = *(int*)((ref *(_003F*)(num6 * 348)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8))));
				num5++;
			}
			while (num5 < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)));
		}
		if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) != 0)
		{
			return false;
		}
		num5 = 0u;
		if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)))
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num8);
			do
			{
				long num7 = (long)num5 * 336L;
				iT0X = *(int*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 16))));
				if (*(int*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 12)))) == 1)
				{
					if (getGraphicsVfMaxkHz((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &num8))
					{
						uiT0Y = num8;
					}
					iT1X = *(int*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 100))));
					uiT1Y = *(uint*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 96))));
					uiT1OCY = *(uint*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 152))));
					iT2X = *(int*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 120))));
					uiT2Y = *(uint*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 116))));
					uiT2OCY = *(uint*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 156))));
					iT3X = *(int*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 140))));
					uiT3Y = *(uint*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 136))));
					uiT3OCY = *(uint*)((ref *(_003F*)num7) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 160))));
					result = true;
				}
				num5++;
			}
			while (num5 < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)));
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool setThermalPff(NvPhysicalGpuHandle__* hGpu, NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID policyId, NV_GPU_CLIENT_PFF_CURVE_V1* pPffCurve, float tempCap)
	{
		//IL_000e: Expected I4, but got I8
		//IL_001c: Expected I4, but got I8
		//IL_015d: Expected I, but got I8
		//IL_017b: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3 nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 0, 1352);
		*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
		if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo(hGpu, &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) != 0)
		{
			return false;
		}
		if ((byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) == 0)
		{
			return false;
		}
		uint num = 0u;
		uint num2 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
		if (0 < num2)
		{
			do
			{
				if (*(int*)((ref *(_003F*)((long)num * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8)))) != (int)policyId)
				{
					num++;
					continue;
				}
				if (*(int*)((ref *(_003F*)((long)num * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 12)))) == 1)
				{
					break;
				}
				return false;
			}
			while (num < num2);
		}
		if (num == System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5)))
		{
			return false;
		}
		*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) = 197960;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)) = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
		num = 0u;
		long num3;
		if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5)))
		{
			do
			{
				num3 = num;
				*(int*)((ref *(_003F*)(num3 * 336)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8)))) = *(int*)((ref *(_003F*)(num3 * 348)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8))));
				num++;
			}
			while (num < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)));
		}
		if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetStatus(hGpu, &nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) != 0)
		{
			return false;
		}
		num = 0u;
		uint num4 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
		if (0 < num4)
		{
			do
			{
				if (*(int*)((ref *(_003F*)((long)num * 336L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8)))) != (int)policyId)
				{
					num++;
					continue;
				}
				if (*(int*)((ref *(_003F*)((long)num * 336L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 12)))) == 1)
				{
					break;
				}
				return false;
			}
			while (num < num4);
		}
		num3 = num;
		long num5 = num3 * 348;
		int num6 = (int)((double)tempCap * 256.0);
		int num7 = *(int*)((ref *(_003F*)num5) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 28))));
		if (num6 <= num7 && num6 >= *(int*)((ref *(_003F*)num5) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 20)))))
		{
			uint num8 = 0u;
			NV_GPU_CLIENT_PFF_CURVE_V1* ptr = (NV_GPU_CLIENT_PFF_CURVE_V1*)((ulong)(nint)pPffCurve + 8uL);
			do
			{
				int num9 = *(int*)ptr;
				if (num9 <= num7 && num9 >= num6)
				{
					num8++;
					ptr = (NV_GPU_CLIENT_PFF_CURVE_V1*)((ulong)(nint)ptr + 20uL);
					continue;
				}
				return false;
			}
			while (num8 < 3);
			long num10 = num3 * 336;
			*(int*)((ref *(_003F*)num10) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 16)))) = num6;
			*(sbyte*)((ref *(_003F*)num10) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 88)))) = 1;
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned((ref *(_003F*)num10) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 92))), pPffCurve, 60);
			return global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesSetStatus(hGpu, &nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) == (_NvAPI_Status)0;
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool setThermalPff(uint nIndex, int iT0X, int iT1X, uint uiT1Y, int iT2X, uint uiT2Y, int iT3X, uint uiT3Y)
	{
		//IL_0064: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_PFF_CURVE_V1 nV_GPU_CLIENT_PFF_CURVE_V);
		*(int*)(&nV_GPU_CLIENT_PFF_CURVE_V) = 1;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_PFF_CURVE_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_PFF_CURVE_V, 20)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_PFF_CURVE_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_PFF_CURVE_V, 40)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_PFF_CURVE_V1, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_PFF_CURVE_V, 4)) = uiT1Y;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_PFF_CURVE_V1, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_PFF_CURVE_V, 24)) = uiT2Y;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_PFF_CURVE_V1, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_PFF_CURVE_V, 44)) = uiT3Y;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_PFF_CURVE_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_PFF_CURVE_V, 8)) = iT1X * 256;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_PFF_CURVE_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_PFF_CURVE_V, 28)) = iT2X * 256;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_PFF_CURVE_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_PFF_CURVE_V, 48)) = iT3X * 256;
		return setThermalPff((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID)1, &nV_GPU_CLIENT_PFF_CURVE_V, iT0X);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getFanCoolerInfo(NvPhysicalGpuHandle__* hGpu, uint nFanIndex, uint* pulMinFanTachometer, uint* pulMaxFanTachometer)
	{
		//IL_0010: Expected I4, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_INFO_V1 nV_GPU_CLIENT_FAN_COOLERS_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 0, 1580);
		*(int*)(&nV_GPU_CLIENT_FAN_COOLERS_INFO_V) = 67116;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanCoolersGetInfo(hGpu, &nV_GPU_CLIENT_FAN_COOLERS_INFO_V) != 0)
		{
			return false;
		}
		if ((System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 4)) & 1) == 0)
		{
			return false;
		}
		if (nFanIndex < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 8)))
		{
			long num = (long)nFanIndex * 48L;
			if ((*(int*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 48)))) & 1) != 0)
			{
				*pulMinFanTachometer = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 52))));
				*pulMaxFanTachometer = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 56))));
				result = true;
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getFanCoolerInfo(uint nIndex, uint nFanIndex, ref bool tachSupported, ref uint rpmMin, ref uint rpmMax)
	{
		//IL_0010: Expected I4, but got I8
		//IL_0027: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_INFO_V1 nV_GPU_CLIENT_FAN_COOLERS_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 0, 1580);
		*(int*)(&nV_GPU_CLIENT_FAN_COOLERS_INFO_V) = 67116;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanCoolersGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_COOLERS_INFO_V) != 0)
		{
			return false;
		}
		if ((System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 4)) & 1) == 0)
		{
			return false;
		}
		if (nFanIndex < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 8)))
		{
			long num = (long)nFanIndex * 48L;
			int num2 = *(int*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 48)))) & 1;
			tachSupported = (byte)num2 != 0;
			rpmMin = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 52))));
			rpmMax = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 56))));
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getFanCoolerStatus(uint nIndex, uint nFanIndex, ref uint nRpmCurr, ref uint nMin, ref uint nMax, ref uint nTarget, ref uint nCoolers)
	{
		//IL_000e: Expected I4, but got I8
		//IL_001c: Expected I4, but got I8
		//IL_0033: Expected I, but got I8
		//IL_005f: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_INFO_V1 nV_GPU_CLIENT_FAN_COOLERS_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 0, 1580);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_STATUS_V1 nV_GPU_CLIENT_FAN_COOLERS_STATUS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_COOLERS_STATUS_V, 0, 1704);
		*(int*)(&nV_GPU_CLIENT_FAN_COOLERS_INFO_V) = 67116;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanCoolersGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_COOLERS_INFO_V) != 0)
		{
			return false;
		}
		if ((System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 4)) & 1) == 0)
		{
			return false;
		}
		*(int*)(&nV_GPU_CLIENT_FAN_COOLERS_STATUS_V) = 67240;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_STATUS_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_STATUS_V, 4)) = 0;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanCoolersGetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_COOLERS_STATUS_V) != 0)
		{
			return false;
		}
		if (nFanIndex < (nCoolers = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 8))))
		{
			long num = (long)nFanIndex * 52L;
			nRpmCurr = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_STATUS_V, 44))));
			nMin = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_STATUS_V, 48))));
			nMax = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_STATUS_V, 52))));
			nTarget = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_STATUS_V, 56))));
		}
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getFanCoolerControl(uint nIndex, uint nFanIndex, ref uint nTarget, ref bool bAuto)
	{
		//IL_000e: Expected I4, but got I8
		//IL_001c: Expected I4, but got I8
		//IL_0033: Expected I, but got I8
		//IL_005f: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_INFO_V1 nV_GPU_CLIENT_FAN_COOLERS_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 0, 1580);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1 nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 0, 1452);
		*(int*)(&nV_GPU_CLIENT_FAN_COOLERS_INFO_V) = 67116;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanCoolersGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_COOLERS_INFO_V) != 0)
		{
			return false;
		}
		if ((System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 4)) & 1) == 0)
		{
			return false;
		}
		*(int*)(&nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V) = 66988;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 8)) = 0;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanCoolersGetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V) != 0)
		{
			return false;
		}
		if (nFanIndex < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 8)))
		{
			long num = (long)nFanIndex * 44L;
			nTarget = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 48))));
			int num2 = ~(*(int*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 52))))) & 1;
			bAuto = (byte)num2 != 0;
		}
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool setCooling(uint nIndex, float level, uint nFanIndex)
	{
		//IL_001b: Expected I4, but got I8
		//IL_0029: Expected I4, but got I8
		//IL_0040: Expected I, but got I8
		//IL_0057: Expected I4, but got I8
		//IL_007d: Expected I, but got I8
		//IL_00f7: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_COOLER_ID nV_GPU_CLIENT_FAN_COOLERS_COOLER_ID);
		switch (nFanIndex)
		{
		case 1u:
			nV_GPU_CLIENT_FAN_COOLERS_COOLER_ID = (_NV_GPU_CLIENT_FAN_COOLERS_COOLER_ID)2;
			break;
		case 0u:
			nV_GPU_CLIENT_FAN_COOLERS_COOLER_ID = (_NV_GPU_CLIENT_FAN_COOLERS_COOLER_ID)1;
			break;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_INFO_V1 nV_GPU_CLIENT_FAN_COOLERS_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 0, 1580);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1 nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 0, 1452);
		*(int*)(&nV_GPU_CLIENT_FAN_COOLERS_INFO_V) = 67116;
		switch (global::_003CModule_003E.NvAPI_GPU_ClientFanCoolersGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_COOLERS_INFO_V))
		{
		case (_NvAPI_Status)(-104):
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_SETCOOLER_LEVEL_V2 nV_GPU_SETCOOLER_LEVEL_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_SETCOOLER_LEVEL_V, 0, 164);
			*(int*)(&nV_GPU_SETCOOLER_LEVEL_V) = 65700;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_SETCOOLER_LEVEL_V2, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_SETCOOLER_LEVEL_V, 4)) = (uint)(double)level;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_SETCOOLER_LEVEL_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_SETCOOLER_LEVEL_V, 8)) = 1;
			return global::_003CModule_003E.NvAPI_GPU_SetCoolerLevels((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], 0u, &nV_GPU_SETCOOLER_LEVEL_V) == (_NvAPI_Status)0;
		}
		default:
			return false;
		case (_NvAPI_Status)0:
		{
			if ((System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 4)) & 1) == 0)
			{
				return false;
			}
			uint num = 0u;
			uint num2 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 8));
			if (0 < num2)
			{
				while (*(int*)((ref *(_003F*)((long)num * 48L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 44)))) != (int)nV_GPU_CLIENT_FAN_COOLERS_COOLER_ID)
				{
					num++;
					if (num >= num2)
					{
						break;
					}
				}
			}
			if (num == System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 8)))
			{
				return false;
			}
			*(int*)(&nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V) = 66988;
			System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 8)) = 1;
			System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1, _NV_GPU_CLIENT_FAN_COOLERS_COOLER_ID>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 44)) = nV_GPU_CLIENT_FAN_COOLERS_COOLER_ID;
			System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 48)) = (uint)(double)level;
			System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 52)) |= 1;
			return global::_003CModule_003E.NvAPI_GPU_ClientFanCoolersSetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V) == (_NvAPI_Status)0;
		}
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool restoreCoolerSettings(uint nIndex)
	{
		//IL_000e: Expected I4, but got I8
		//IL_001c: Expected I4, but got I8
		//IL_0033: Expected I, but got I8
		//IL_006f: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_INFO_V1 nV_GPU_CLIENT_FAN_COOLERS_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 0, 1580);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1 nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 0, 1452);
		*(int*)(&nV_GPU_CLIENT_FAN_COOLERS_INFO_V) = 67116;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanCoolersGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_COOLERS_INFO_V) != 0)
		{
			return false;
		}
		if ((System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 4)) & 1) == 0)
		{
			return false;
		}
		*(int*)(&nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V) = 66988;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 8)) = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_INFO_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_INFO_V, 8));
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_COOLERS_CONTROL_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V, 4)) |= 1;
		return global::_003CModule_003E.NvAPI_GPU_ClientFanCoolersSetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_COOLERS_CONTROL_V) == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getFanCurve(uint nIndex, byte policyIdx, ref int tj0, ref uint pwm0, ref uint rpm0, ref int tj1, ref uint pwm1, ref uint rpm1, ref int tj2, ref uint pwm2, ref uint rpm2, [MarshalAs(UnmanagedType.U1)] bool bDefault)
	{
		//IL_000d: Expected I4, but got I8
		//IL_001b: Expected I4, but got I8
		//IL_0032: Expected I, but got I8
		//IL_0088: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_POLICIES_INFO_V2 nV_GPU_CLIENT_FAN_POLICIES_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_POLICIES_INFO_V, 0, 76);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2 nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 0, 220);
		*(int*)(&nV_GPU_CLIENT_FAN_POLICIES_INFO_V) = 131148;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_POLICIES_INFO_V) != 0)
		{
			return false;
		}
		if ((System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_INFO_V, 4)) & 1) == 0)
		{
			return false;
		}
		*(int*)(&nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V) = 131292;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 4)) = 0;
		byte b = (byte)(bDefault ? 1 : 0);
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 216)) ^= (b ^ System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 216))) & 1;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanPoliciesGetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V) != 0)
		{
			return false;
		}
		if ((uint)policyIdx < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 4)))
		{
			long num = (long)policyIdx * 52L;
			tj0 = (int)((double)(*(int*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 24))))) * (1.0 / 256.0));
			pwm0 = (uint)((double)(*(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 28))))) * 100.0 * 1.52587890625E-05);
			rpm0 = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 32))));
			tj1 = (int)((double)(*(int*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 36))))) * (1.0 / 256.0));
			pwm1 = (uint)((double)(*(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 40))))) * 100.0 * 1.52587890625E-05);
			rpm1 = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 44))));
			tj2 = (int)((double)(*(int*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 48))))) * (1.0 / 256.0));
			pwm2 = (uint)((double)(*(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 52))))) * 100.0 * 1.52587890625E-05);
			rpm2 = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 56))));
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool getFanCurve(uint nIndex, byte policyIdx, ref int tj0, ref uint pwm0, ref uint rpm0, ref int tj1, ref uint pwm1, ref uint rpm1, ref int tj2, ref uint pwm2, ref uint rpm2)
	{
		return getFanCurve(nIndex, policyIdx, ref tj0, ref pwm0, ref rpm0, ref tj1, ref pwm1, ref rpm1, ref tj2, ref pwm2, ref rpm2, bDefault: false);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool setFanCurve(uint nIndex, byte policyIdx, int tj0, uint pwm0, uint rpm0, int tj1, uint pwm1, uint rpm1, int tj2, uint pwm2, uint rpm2)
	{
		//IL_000b: Expected I4, but got I8
		//IL_0019: Expected I4, but got I8
		//IL_0030: Expected I, but got I8
		//IL_00d7: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_POLICIES_INFO_V2 nV_GPU_CLIENT_FAN_POLICIES_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_POLICIES_INFO_V, 0, 76);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2 nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 0, 220);
		*(int*)(&nV_GPU_CLIENT_FAN_POLICIES_INFO_V) = 131148;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanPoliciesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_POLICIES_INFO_V) != 0)
		{
			return false;
		}
		if ((System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_INFO_V, 4)) & 1) == 0)
		{
			return false;
		}
		*(int*)(&nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V) = 131292;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 4)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 20)) = policyIdx;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 24)) = tj0 * 256;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 28)) = pwm0 * 65536 / 100;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 32)) = rpm0;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 36)) = tj1 * 256;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 40)) = pwm1 * 65536 / 100;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 44)) = rpm1;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 48)) = tj2 * 256;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 52)) = pwm2 * 65536 / 100;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_POLICIES_CONTROL_V2, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V, 56)) = rpm2;
		return global::_003CModule_003E.NvAPI_GPU_ClientFanPoliciesSetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_POLICIES_CONTROL_V) == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public bool setFanCurveDefault(uint nIndex, byte policyIdx)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out int tj);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint pwm);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint rpm);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out int tj2);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint pwm2);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint rpm2);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out int tj3);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint pwm3);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint rpm3);
		bool flag = getFanCurve(nIndex, policyIdx, ref tj, ref pwm, ref rpm, ref tj2, ref pwm2, ref rpm2, ref tj3, ref pwm3, ref rpm3, bDefault: true);
		if (flag)
		{
			flag = setFanCurve(nIndex, policyIdx, tj, pwm, rpm, tj2, pwm2, rpm2, tj3, pwm3, rpm3);
		}
		return flag;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getFanArbiterInfo(uint nIndex, byte arbiterIdx, ref bool bFanStopSupported, ref bool bFanStopDefaultEnable)
	{
		//IL_0010: Expected I4, but got I8
		//IL_0027: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_ARBITERS_INFO_V1 nV_GPU_CLIENT_FAN_ARBITERS_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_ARBITERS_INFO_V, 0, 1316);
		*(int*)(&nV_GPU_CLIENT_FAN_ARBITERS_INFO_V) = 66852;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanArbitersGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_ARBITERS_INFO_V) != 0)
		{
			return false;
		}
		if ((uint)arbiterIdx < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_INFO_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_INFO_V, 4)))
		{
			uint num = *(uint*)((ref *(_003F*)((long)arbiterIdx * 40L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_INFO_V, 40))));
			int num2 = (int)(num & 1);
			bFanStopSupported = (byte)num2 != 0;
			int num3 = (int)((num >> 1) & 1);
			bFanStopDefaultEnable = (byte)num3 != 0;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getFanArbiterStatus(uint nIndex, byte arbiterIdx, ref bool fanStopActive)
	{
		//IL_0010: Expected I4, but got I8
		//IL_002d: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_ARBITERS_STATUS_V1 nV_GPU_CLIENT_FAN_ARBITERS_STATUS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_ARBITERS_STATUS_V, 0, 292);
		*(int*)(&nV_GPU_CLIENT_FAN_ARBITERS_STATUS_V) = 65828;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_STATUS_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_STATUS_V, 4)) = 0;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanArbitersGetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_ARBITERS_STATUS_V) != 0)
		{
			return false;
		}
		if ((uint)arbiterIdx < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_STATUS_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_STATUS_V, 4)))
		{
			int num = *(int*)((ref *(_003F*)((long)arbiterIdx * 8L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_STATUS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_STATUS_V, 40)))) & 1;
			fanStopActive = (byte)num != 0;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getFanArbiterControl(uint nIndex, byte arbiterIdx, ref bool fanStopFeatureEnable)
	{
		//IL_0010: Expected I4, but got I8
		//IL_002d: Expected I, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V1 nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V, 0, 292);
		*(int*)(&nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V) = 65828;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V, 4)) = 0;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanArbitersGetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V) != 0)
		{
			return false;
		}
		if ((uint)arbiterIdx < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V, 4)))
		{
			int num = *(int*)((ref *(_003F*)((long)arbiterIdx * 8L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V, 40)))) & 1;
			fanStopFeatureEnable = (byte)num != 0;
			result = true;
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool setFanArbiterControl(uint nIndex, byte arbiterIdx, [MarshalAs(UnmanagedType.U1)] bool fanStopFeatureEnable)
	{
		//IL_000e: Expected I4, but got I8
		//IL_001c: Expected I4, but got I8
		//IL_0033: Expected I, but got I8
		//IL_00b7: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_ARBITERS_INFO_V1 nV_GPU_CLIENT_FAN_ARBITERS_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_ARBITERS_INFO_V, 0, 1316);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V1 nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V, 0, 292);
		*(int*)(&nV_GPU_CLIENT_FAN_ARBITERS_INFO_V) = 66852;
		if (global::_003CModule_003E.NvAPI_GPU_ClientFanArbitersGetInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_ARBITERS_INFO_V) != 0)
		{
			return false;
		}
		uint num = 0u;
		uint num2 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_INFO_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_INFO_V, 4));
		if (0 < num2)
		{
			do
			{
				long num3 = (long)num * 40L;
				if (*(byte*)((ref *(_003F*)num3) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_INFO_V, 44)))) == arbiterIdx && (*(int*)((ref *(_003F*)num3) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_INFO_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_INFO_V, 40)))) & 1) != 0)
				{
					break;
				}
				num++;
			}
			while (num < num2);
		}
		if (num == System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_INFO_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_INFO_V, 4)))
		{
			return false;
		}
		*(int*)(&nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V) = 65828;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V1, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V, 4)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V1, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V, 36)) = arbiterIdx;
		int num4 = (fanStopFeatureEnable ? 1 : 0);
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V, 40)) ^= (System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V, 40)) ^ num4) & 1;
		global::_003CModule_003E.NvAPI_GPU_ClientFanArbitersSetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_FAN_ARBITERS_CONTROL_V);
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getClientClkStatus(uint nIndex, ValueType ClrVfPointsStatus, ref int nLength)
	{
		//IL_000a: Expected I, but got I8
		//IL_0069: Expected I, but got I8
		NvPhysicalGpuHandle__* ptr = (NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex];
		System.Runtime.CompilerServices.Unsafe.SkipInit(out ManagedNvApi.CLIENT_CLK_INFO cLIENT_CLK_INFO);
		*(int*)(&cLIENT_CLK_INFO) = 67880;
		if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkDomainsGetInfo(ptr, (_NV_GPU_CLOCK_CLIENT_CLK_DOMAINS_INFO_V1*)(&cLIENT_CLK_INFO)) == (_NvAPI_Status)0)
		{
			System.Runtime.CompilerServices.Unsafe.As<ManagedNvApi.CLIENT_CLK_INFO, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2344)) = 71724;
			if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetInfo(ptr, (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_INFO_V1*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2344))) == (_NvAPI_Status)0)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out CLIENT_CLK_VF_POINTS_STATUS cLIENT_CLK_VF_POINTS_STATUS);
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_VF_POINTS_STATUS, 4), ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2348), 32);
				if (clientClkVfPointsGetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &cLIENT_CLK_VF_POINTS_STATUS) != 0)
				{
					return false;
				}
				uint num = 0u;
				ulong num2 = 0uL;
				long num3 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 40));
				do
				{
					if (0 != ((1 << (int)num) & *(int*)((ref *(_003F*)((num2 >> 5) * 4)) + (ref System.Runtime.CompilerServices.Unsafe.As<ManagedNvApi.CLIENT_CLK_INFO, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 4))))) && 0 == *(int*)num3)
					{
						uint num4 = *(byte*)(num3 + 48);
						if (num4 <= *(byte*)(num3 + 49))
						{
							do
							{
								if (*(int*)(&cLIENT_CLK_VF_POINTS_STATUS) != 72744 && *(int*)(&cLIENT_CLK_VF_POINTS_STATUS) != 138280)
								{
									if (*(int*)(&cLIENT_CLK_VF_POINTS_STATUS) == 219916)
									{
										_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_STATUS_V3* ptr2 = (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_STATUS_V3*)((ref *(_003F*)((long)num4 * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<CLIENT_CLK_VF_POINTS_STATUS, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_VF_POINTS_STATUS, 104))));
										if (num4 < (uint)((VfPointsStatus)ClrVfPointsStatus).numOffsets)
										{
											if (System.Runtime.CompilerServices.Unsafe.As<CLIENT_CLK_VF_POINTS_STATUS, bool>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_VF_POINTS_STATUS, 36)))
											{
												((VfPointsStatus)ClrVfPointsStatus).offsets[num4].zeroedFrequencyKhz = *(int*)((ulong)(nint)ptr2 + 12uL);
											}
											((VfPointsStatus)ClrVfPointsStatus).offsets[num4].millivoltage = *(int*)((ulong)(nint)ptr2 + 56uL);
											((VfPointsStatus)ClrVfPointsStatus).offsets[num4].overclockedFrequencyKhz = *(int*)((ulong)(nint)ptr2 + 52uL);
										}
									}
								}
								else
								{
									_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_STATUS_V1* ptr3 = (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_STATUS_V1*)((ref *(_003F*)((long)num4 * 28L)) + (ref System.Runtime.CompilerServices.Unsafe.As<CLIENT_CLK_VF_POINTS_STATUS, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_VF_POINTS_STATUS, 68))));
									if (num4 < (uint)((VfPointsStatus)ClrVfPointsStatus).numOffsets)
									{
										((VfPointsStatus)ClrVfPointsStatus).offsets[num4].millivoltage = *(int*)((ulong)(nint)ptr3 + 8uL);
										((VfPointsStatus)ClrVfPointsStatus).offsets[num4].overclockedFrequencyKhz = *(int*)((ulong)(nint)ptr3 + 4uL);
									}
								}
								num4++;
							}
							while (num4 <= *(byte*)(num3 + 49));
						}
						if (*(int*)(num3 + 4) == 0)
						{
							nLength = (int)num4;
						}
					}
					num++;
					num2++;
					num3 += 72;
				}
				while (num < 32);
				return true;
			}
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getClientClkControl(uint nIndex, ValueType ClrVfPointsStatus, ref int nLength)
	{
		//IL_000a: Expected I, but got I8
		//IL_0070: Expected I, but got I8
		//IL_00c7: Expected I, but got I8
		NvPhysicalGpuHandle__* ptr = (NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex];
		System.Runtime.CompilerServices.Unsafe.SkipInit(out ManagedNvApi.CLIENT_CLK_INFO cLIENT_CLK_INFO);
		*(int*)(&cLIENT_CLK_INFO) = 67880;
		if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkDomainsGetInfo(ptr, (_NV_GPU_CLOCK_CLIENT_CLK_DOMAINS_INFO_V1*)(&cLIENT_CLK_INFO)) == (_NvAPI_Status)0)
		{
			System.Runtime.CompilerServices.Unsafe.As<ManagedNvApi.CLIENT_CLK_INFO, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2344)) = 71724;
			if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetInfo(ptr, (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_INFO_V1*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2344))) == (_NvAPI_Status)0)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V1 nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V);
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V, 4), ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2348), 32);
				*(int*)(&nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V) = 140320;
				if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V) != 0)
				{
					return false;
				}
				uint num = 0u;
				ulong num2 = 0uL;
				long num3 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 40));
				do
				{
					if (0 != ((1 << (int)num) & *(int*)((ref *(_003F*)((num2 >> 5) * 4)) + (ref System.Runtime.CompilerServices.Unsafe.As<ManagedNvApi.CLIENT_CLK_INFO, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 4))))) && 0 == *(int*)num3)
					{
						uint num4 = *(byte*)(num3 + 48);
						if (num4 <= *(byte*)(num3 + 49))
						{
							do
							{
								_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_CONTROL_V1* ptr2 = (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_CONTROL_V1*)((ref *(_003F*)((long)num4 * 36L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V, 68))));
								if (0 == *(int*)ptr2)
								{
									_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_CONTROL_PROG_V1* ptr3 = (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_CONTROL_PROG_V1*)((ulong)(nint)ptr2 + 20uL);
									if (num4 < (uint)((VfPointsStatus)ClrVfPointsStatus).numOffsets)
									{
										((VfPointsStatus)ClrVfPointsStatus).offsets[num4].offsetFrequencyKhz = *(int*)ptr3;
									}
								}
								num4++;
							}
							while (num4 <= *(byte*)(num3 + 49));
						}
						if (*(int*)(num3 + 4) == 0)
						{
							nLength = (int)num4;
						}
					}
					num++;
					num2++;
					num3 += 72;
				}
				while (num < 32);
				return true;
			}
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool setClientClkVfPointOffset(uint nIndex, ValueType ClrVfPointsStatus)
	{
		//IL_000a: Expected I, but got I8
		//IL_0070: Expected I, but got I8
		//IL_0121: Expected I, but got I8
		//IL_00cb: Expected I, but got I8
		NvPhysicalGpuHandle__* ptr = (NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex];
		System.Runtime.CompilerServices.Unsafe.SkipInit(out ManagedNvApi.CLIENT_CLK_INFO cLIENT_CLK_INFO);
		*(int*)(&cLIENT_CLK_INFO) = 67880;
		if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkDomainsGetInfo(ptr, (_NV_GPU_CLOCK_CLIENT_CLK_DOMAINS_INFO_V1*)(&cLIENT_CLK_INFO)) == (_NvAPI_Status)0)
		{
			System.Runtime.CompilerServices.Unsafe.As<ManagedNvApi.CLIENT_CLK_INFO, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2344)) = 71724;
			if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetInfo(ptr, (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_INFO_V1*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2344))) == (_NvAPI_Status)0)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V1 nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V);
				*(int*)(&nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V) = 140320;
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V, 4), ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2348), 32);
				if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V) != 0)
				{
					return false;
				}
				uint num = 0u;
				ulong num2 = 0uL;
				long num3 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 40));
				do
				{
					if (0 != ((1 << (int)num) & *(int*)((ref *(_003F*)((num2 >> 5) * 4)) + (ref System.Runtime.CompilerServices.Unsafe.As<ManagedNvApi.CLIENT_CLK_INFO, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 4))))) && 0 == *(int*)num3)
					{
						uint num4 = *(byte*)(num3 + 48);
						uint num5 = *(byte*)(num3 + 49);
						if (num4 <= num5)
						{
							do
							{
								_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_CONTROL_V1* ptr2 = (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_CONTROL_V1*)((ref *(_003F*)((long)num4 * 36L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V, 68))));
								if (0 == *(int*)ptr2)
								{
									_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_CONTROL_PROG_V1* ptr3 = (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINT_CONTROL_PROG_V1*)((ulong)(nint)ptr2 + 20uL);
									if (num4 < (uint)((VfPointsStatus)ClrVfPointsStatus).numOffsets)
									{
										*(int*)ptr3 = ((VfPointsStatus)ClrVfPointsStatus).offsets[num4].offsetFrequencyKhz;
									}
								}
								num4++;
							}
							while (num4 <= num5);
						}
					}
					num++;
					num2++;
					num3 += 72;
				}
				while (num < 32);
				_NvAPI_Status nvAPI_Status = global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsSetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_CONTROL_V);
				if (nvAPI_Status != 0)
				{
					int num6 = ((nvAPI_Status == (_NvAPI_Status)(-217)) ? 1 : 0);
					isGpuInDebugMode = (byte)num6 != 0;
					return false;
				}
				return true;
			}
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getLedZoneType(uint nIndex, uint nZoneIndex, ref NV_GPU_CLIENT_ILLUM_ZONE_TYPE type, ref NV_GPU_CLIENT_ILLUM_ZONE_LOCATION location)
	{
		//IL_001c: Expected I4, but got I8
		//IL_002e: Expected I, but got I8
		NvPhysicalGpuHandle__*[] array = gpuHandles;
		if ((nuint)(int)nIndex >= (nuint)array.LongLength)
		{
			return false;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_ILLUM_ZONE_INFO_PARAMS_V1 nV_GPU_CLIENT_ILLUM_ZONE_INFO_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_ILLUM_ZONE_INFO_PARAMS_V, 0, 4552);
		*(int*)(&nV_GPU_CLIENT_ILLUM_ZONE_INFO_PARAMS_V) = 70088;
		if (global::_003CModule_003E.NvAPI_GPU_ClientIllumZonesGetInfo((NvPhysicalGpuHandle__*)((long[])(object)array)[nIndex], &nV_GPU_CLIENT_ILLUM_ZONE_INFO_PARAMS_V) != 0)
		{
			return false;
		}
		if (nZoneIndex >= (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_INFO_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_INFO_PARAMS_V, 4)))
		{
			return false;
		}
		_NV_GPU_CLIENT_ILLUM_ZONE_INFO_V1* ptr = (_NV_GPU_CLIENT_ILLUM_ZONE_INFO_V1*)((ref *(_003F*)((long)nZoneIndex * 140L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_INFO_PARAMS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_INFO_PARAMS_V, 72))));
		type = *(NV_GPU_CLIENT_ILLUM_ZONE_TYPE*)ptr;
		location = *(NV_GPU_CLIENT_ILLUM_ZONE_LOCATION*)((ulong)(nint)ptr + 8uL);
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool ClientIllumZonesGetControl(uint nIndex, uint nZoneIndex, ref byte red, ref byte green, ref byte blue, ref byte brightness)
	{
		//IL_001c: Expected I4, but got I8
		//IL_003b: Expected I, but got I8
		NvPhysicalGpuHandle__*[] array = gpuHandles;
		if ((nuint)(int)nIndex >= (nuint)array.LongLength)
		{
			return false;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1 nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 0, 6476);
		*(int*)(&nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V) = 72012;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 4)) &= -2;
		if (global::_003CModule_003E.NvAPI_GPU_ClientIllumZonesGetControl((NvPhysicalGpuHandle__*)((long[])(object)array)[nIndex], &nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V) != 0)
		{
			return false;
		}
		if (nZoneIndex >= (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 8)))
		{
			return false;
		}
		_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_V1* ptr = (_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_V1*)((ref *(_003F*)((long)nZoneIndex * 200L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 76))));
		switch (*(int*)ptr)
		{
		case 4:
			brightness = *(byte*)((ulong)(nint)ptr + 8uL);
			break;
		case 3:
		{
			red = *(byte*)((ulong)(nint)ptr + 8uL);
			green = *(byte*)((ulong)(nint)ptr + 9uL);
			blue = *(byte*)((ulong)(nint)ptr + 10uL);
			byte b = *(byte*)((ulong)(nint)ptr + 11uL);
			if (b != 0)
			{
				blue = b;
				green = b;
				red = b;
			}
			brightness = *(byte*)((ulong)(nint)ptr + 12uL);
			break;
		}
		case 2:
			brightness = *(byte*)((ulong)(nint)ptr + 8uL);
			break;
		case 1:
			red = *(byte*)((ulong)(nint)ptr + 8uL);
			green = *(byte*)((ulong)(nint)ptr + 9uL);
			blue = *(byte*)((ulong)(nint)ptr + 10uL);
			brightness = *(byte*)((ulong)(nint)ptr + 11uL);
			break;
		}
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool ClientIllumZonesSetControl(uint nIndex, uint nZoneIndex, byte red0, byte green0, byte blue0, byte brightnessPercent0, byte red1, byte green1, byte blue1, byte brightnessPercent1, NV_GPU_CLIENT_ILLUM_PIECEWISE_LINEAR_CYCLE_TYPE cycleType, byte grpCount, ushort riseTimems, ushort fallTimems, ushort ATimems, ushort BTimems, ushort grpIdleTimems, ushort phaseOffsetms)
	{
		//IL_001c: Expected I4, but got I8
		//IL_003b: Expected I, but got I8
		//IL_025f: Expected I, but got I8
		NvPhysicalGpuHandle__*[] array = gpuHandles;
		if ((nuint)(int)nIndex >= (nuint)array.LongLength)
		{
			return false;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1 nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 0, 6476);
		*(int*)(&nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V) = 72012;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 4)) &= -2;
		if (global::_003CModule_003E.NvAPI_GPU_ClientIllumZonesGetControl((NvPhysicalGpuHandle__*)((long[])(object)array)[nIndex], &nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V) != 0)
		{
			return false;
		}
		if (nZoneIndex >= (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 8)))
		{
			return false;
		}
		_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_V1* ptr = (_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_V1*)((ref *(_003F*)((long)nZoneIndex * 200L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 76))));
		*(int*)((ulong)(nint)ptr + 4uL) = 1;
		byte white = 0;
		switch (*(int*)ptr)
		{
		case 4:
			*(byte*)((ulong)(nint)ptr + 8uL) = brightnessPercent0;
			*(byte*)((ulong)(nint)ptr + 9uL) = brightnessPercent1;
			*(NV_GPU_CLIENT_ILLUM_PIECEWISE_LINEAR_CYCLE_TYPE*)((ulong)(nint)ptr + 12uL) = cycleType;
			*(byte*)((ulong)(nint)ptr + 16uL) = grpCount;
			*(ushort*)((ulong)(nint)ptr + 18uL) = riseTimems;
			*(ushort*)((ulong)(nint)ptr + 20uL) = fallTimems;
			*(ushort*)((ulong)(nint)ptr + 22uL) = ATimems;
			*(ushort*)((ulong)(nint)ptr + 24uL) = BTimems;
			*(ushort*)((ulong)(nint)ptr + 26uL) = grpIdleTimems;
			*(ushort*)((ulong)(nint)ptr + 28uL) = phaseOffsetms;
			break;
		case 3:
			CheckRGBW(ref red0, ref green0, ref blue0, ref white);
			*(byte*)((ulong)(nint)ptr + 8uL) = red0;
			*(byte*)((ulong)(nint)ptr + 9uL) = green0;
			*(byte*)((ulong)(nint)ptr + 10uL) = blue0;
			*(byte*)((ulong)(nint)ptr + 11uL) = white;
			*(byte*)((ulong)(nint)ptr + 12uL) = brightnessPercent0;
			CheckRGBW(ref red1, ref green1, ref blue1, ref white);
			*(byte*)((ulong)(nint)ptr + 13uL) = red1;
			*(byte*)((ulong)(nint)ptr + 14uL) = green1;
			*(byte*)((ulong)(nint)ptr + 15uL) = blue1;
			*(byte*)((ulong)(nint)ptr + 16uL) = white;
			*(byte*)((ulong)(nint)ptr + 17uL) = brightnessPercent1;
			*(NV_GPU_CLIENT_ILLUM_PIECEWISE_LINEAR_CYCLE_TYPE*)((ulong)(nint)ptr + 20uL) = cycleType;
			*(byte*)((ulong)(nint)ptr + 24uL) = grpCount;
			*(ushort*)((ulong)(nint)ptr + 26uL) = riseTimems;
			*(ushort*)((ulong)(nint)ptr + 28uL) = fallTimems;
			*(ushort*)((ulong)(nint)ptr + 30uL) = ATimems;
			*(ushort*)((ulong)(nint)ptr + 32uL) = BTimems;
			*(ushort*)((ulong)(nint)ptr + 34uL) = grpIdleTimems;
			*(ushort*)((ulong)(nint)ptr + 36uL) = phaseOffsetms;
			break;
		case 2:
			*(byte*)((ulong)(nint)ptr + 8uL) = brightnessPercent0;
			*(byte*)((ulong)(nint)ptr + 9uL) = brightnessPercent1;
			*(NV_GPU_CLIENT_ILLUM_PIECEWISE_LINEAR_CYCLE_TYPE*)((ulong)(nint)ptr + 12uL) = cycleType;
			*(byte*)((ulong)(nint)ptr + 16uL) = grpCount;
			*(ushort*)((ulong)(nint)ptr + 18uL) = riseTimems;
			*(ushort*)((ulong)(nint)ptr + 20uL) = fallTimems;
			*(ushort*)((ulong)(nint)ptr + 22uL) = ATimems;
			*(ushort*)((ulong)(nint)ptr + 24uL) = BTimems;
			*(ushort*)((ulong)(nint)ptr + 26uL) = grpIdleTimems;
			*(ushort*)((ulong)(nint)ptr + 28uL) = phaseOffsetms;
			break;
		case 1:
			*(byte*)((ulong)(nint)ptr + 8uL) = red0;
			*(byte*)((ulong)(nint)ptr + 9uL) = green0;
			*(byte*)((ulong)(nint)ptr + 10uL) = blue0;
			*(byte*)((ulong)(nint)ptr + 11uL) = brightnessPercent0;
			*(byte*)((ulong)(nint)ptr + 12uL) = red1;
			*(byte*)((ulong)(nint)ptr + 13uL) = green1;
			*(byte*)((ulong)(nint)ptr + 14uL) = blue1;
			*(byte*)((ulong)(nint)ptr + 15uL) = brightnessPercent1;
			*(NV_GPU_CLIENT_ILLUM_PIECEWISE_LINEAR_CYCLE_TYPE*)((ulong)(nint)ptr + 16uL) = cycleType;
			*(byte*)((ulong)(nint)ptr + 20uL) = grpCount;
			*(ushort*)((ulong)(nint)ptr + 22uL) = riseTimems;
			*(ushort*)((ulong)(nint)ptr + 24uL) = fallTimems;
			*(ushort*)((ulong)(nint)ptr + 26uL) = ATimems;
			*(ushort*)((ulong)(nint)ptr + 28uL) = BTimems;
			*(ushort*)((ulong)(nint)ptr + 30uL) = grpIdleTimems;
			*(ushort*)((ulong)(nint)ptr + 32uL) = phaseOffsetms;
			break;
		}
		_NvAPI_Status num = global::_003CModule_003E.NvAPI_GPU_ClientIllumZonesSetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V);
		global::_003CModule_003E.Sleep(30u);
		return num == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool ClientIllumZonesSetControl(uint nIndex, uint nZoneIndex, byte red, byte green, byte blue, byte brightnessPercent)
	{
		//IL_001c: Expected I4, but got I8
		//IL_003b: Expected I, but got I8
		//IL_00e8: Expected I, but got I8
		NvPhysicalGpuHandle__*[] array = gpuHandles;
		if ((nuint)(int)nIndex >= (nuint)array.LongLength)
		{
			return false;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1 nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 0, 6476);
		*(int*)(&nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V) = 72012;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 4)) &= -2;
		if (global::_003CModule_003E.NvAPI_GPU_ClientIllumZonesGetControl((NvPhysicalGpuHandle__*)((long[])(object)array)[nIndex], &nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V) != 0)
		{
			return false;
		}
		if (nZoneIndex >= (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 8)))
		{
			return false;
		}
		_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_V1* ptr = (_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_V1*)((ref *(_003F*)((long)nZoneIndex * 200L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 76))));
		*(int*)((ulong)(nint)ptr + 4uL) = 0;
		byte white = 0;
		switch (*(int*)ptr)
		{
		case 4:
			*(byte*)((ulong)(nint)ptr + 8uL) = brightnessPercent;
			break;
		case 3:
			CheckRGBW(ref red, ref green, ref blue, ref white);
			*(byte*)((ulong)(nint)ptr + 8uL) = red;
			*(byte*)((ulong)(nint)ptr + 9uL) = green;
			*(byte*)((ulong)(nint)ptr + 10uL) = blue;
			*(byte*)((ulong)(nint)ptr + 11uL) = white;
			*(byte*)((ulong)(nint)ptr + 12uL) = brightnessPercent;
			break;
		case 2:
			*(byte*)((ulong)(nint)ptr + 8uL) = brightnessPercent;
			break;
		case 1:
			*(byte*)((ulong)(nint)ptr + 8uL) = red;
			*(byte*)((ulong)(nint)ptr + 9uL) = green;
			*(byte*)((ulong)(nint)ptr + 10uL) = blue;
			*(byte*)((ulong)(nint)ptr + 11uL) = brightnessPercent;
			break;
		}
		_NvAPI_Status num = global::_003CModule_003E.NvAPI_GPU_ClientIllumZonesSetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V);
		global::_003CModule_003E.Sleep(30u);
		return num == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool ClientIllumZonesSetControlDefault(uint nIndex)
	{
		//IL_001c: Expected I4, but got I8
		//IL_003a: Expected I, but got I8
		//IL_004d: Expected I, but got I8
		NvPhysicalGpuHandle__*[] array = gpuHandles;
		if ((nuint)(int)nIndex >= (nuint)array.LongLength)
		{
			return false;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1 nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 0, 6476);
		*(int*)(&nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V) = 72012;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V, 4)) |= 1;
		if (global::_003CModule_003E.NvAPI_GPU_ClientIllumZonesGetControl((NvPhysicalGpuHandle__*)((long[])(object)array)[nIndex], &nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V) != 0)
		{
			return false;
		}
		_NvAPI_Status num = global::_003CModule_003E.NvAPI_GPU_ClientIllumZonesSetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_ILLUM_ZONE_CONTROL_PARAMS_V);
		global::_003CModule_003E.Sleep(30u);
		return num == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool ClientIllumDevicesSetControl(uint nIndex, [MarshalAs(UnmanagedType.U1)] bool sync)
	{
		//IL_001c: Expected I4, but got I8
		//IL_002e: Expected I, but got I8
		//IL_0095: Expected I, but got I8
		NvPhysicalGpuHandle__*[] array = gpuHandles;
		if ((nuint)(int)nIndex >= (nuint)array.LongLength)
		{
			return false;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V1 nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V, 0, 4936);
		*(int*)(&nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V) = 70472;
		if (global::_003CModule_003E.NvAPI_GPU_ClientIllumDevicesGetControl((NvPhysicalGpuHandle__*)((long[])(object)array)[nIndex], &nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V) != 0)
		{
			return false;
		}
		*(int*)(&nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V) = 70472;
		uint num = 0u;
		if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V, 4)))
		{
			do
			{
				if (sync)
				{
					long num2 = (long)num * 152L;
					*(sbyte*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V, 80)))) = 1;
					*(ulong*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V, 88)))) = global::_003CModule_003E.GetTickCount64();
				}
				else
				{
					*(sbyte*)((ref *(_003F*)((long)num * 152L)) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V1, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V, 80)))) = 0;
				}
				num++;
			}
			while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V, 4)));
		}
		return global::_003CModule_003E.NvAPI_GPU_ClientIllumDevicesSetControl((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], &nV_GPU_CLIENT_ILLUM_DEVICE_CONTROL_PARAMS_V) == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getPrioritize(uint nIndex, ref bool bRemoveTdpLimit)
	{
		//IL_0011: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out int num);
		if (getTdpLimit((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID)1, &num))
		{
			byte b = (byte)((num != 0) ? 1 : 0);
			bRemoveTdpLimit = b != 0;
			return true;
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool setPrioritize(uint nIndex, [MarshalAs(UnmanagedType.U1)] bool bRemoveTdpLimit)
	{
		//IL_0010: Expected I, but got I8
		return setTdpLimit((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID)1, bRemoveTdpLimit ? 1 : 0);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getLinkedPowerCap(NvPhysicalGpuHandle__* hGpu, NV_GPU_CLIENT_POWER_POLICIES_POLICY_ID pwrPolicyId, NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID thermPolicyId, float fPower, float* pfTemperature)
	{
		//IL_0011: Expected I4, but got I8
		//IL_001f: Expected I4, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_POLICIES_INFO_V2 nV_GPU_CLIENT_POWER_POLICIES_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 0, 2248);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
		*(int*)(&nV_GPU_CLIENT_POWER_POLICIES_INFO_V) = 133320;
		if (global::_003CModule_003E.NvAPI_GPU_ClientPowerPoliciesGetInfo(hGpu, &nV_GPU_CLIENT_POWER_POLICIES_INFO_V) == (_NvAPI_Status)0 && System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, bool>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 4)))
		{
			int num = 0;
			int num2 = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 5));
			if (0 < num2)
			{
				long num3 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 8));
				do
				{
					if (*(int*)num3 != (int)pwrPolicyId)
					{
						num++;
						num3 += 560;
						continue;
					}
					long num4 = (long)num * 560L;
					float num5 = *(uint*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 48))));
					float num6 = *(uint*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 24))));
					float num7 = *(uint*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 36))));
					double num8 = (double)fPower * 1000.0;
					if (!(num8 <= (double)num5) || !(num8 >= (double)num6))
					{
						break;
					}
					*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
					if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo(hGpu, &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) != 0 || (byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) == 0)
					{
						break;
					}
					int num9 = 0;
					int num10 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
					if (0 >= num10)
					{
						break;
					}
					long num11 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8));
					do
					{
						if (*(int*)num11 != (int)thermPolicyId)
						{
							num9++;
							num11 += 348;
							continue;
						}
						long num12 = (long)num9 * 348L;
						float num13 = (float)(*(int*)((ref *(_003F*)num12) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 28))))) * 0.00390625f;
						float num14 = (float)(*(int*)((ref *(_003F*)num12) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 20))))) * 0.00390625f;
						float num15 = (float)(*(int*)((ref *(_003F*)num12) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 24))))) * 0.00390625f;
						if (num8 > (double)num7)
						{
							float num16 = (*pfTemperature = (float)((num8 - (double)num7) / ((double)num5 - (double)num7) * ((double)num13 - (double)num15) + (double)num15));
							float num17 = ((!((double)num16 > (double)num13)) ? num16 : num13);
							*pfTemperature = num17;
						}
						else
						{
							double num18 = num15;
							float num19 = (*pfTemperature = (float)(num18 - ((double)num7 - num8) / ((double)num7 - (double)num6) * (num18 - (double)num14)));
							float num20 = ((!((double)num19 < (double)num14)) ? num19 : num14);
							*pfTemperature = num20;
						}
						result = true;
						break;
					}
					while (num9 < num10);
					break;
				}
				while (num < num2);
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getLinkedPowerCap(uint nIndex, float fPower, ref float fTemperature)
	{
		//IL_0013: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out float num);
		if (getLinkedPowerCap((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLIENT_POWER_POLICIES_POLICY_ID)0, (NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID)1, fPower, &num))
		{
			fTemperature = num;
			return true;
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getLinkedTempCap(NvPhysicalGpuHandle__* hGpu, NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID thermPolicyId, NV_GPU_CLIENT_POWER_POLICIES_POLICY_ID pwrPolicyId, float fTemperature, float* pfPower)
	{
		//IL_0011: Expected I4, but got I8
		//IL_001f: Expected I4, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLIENT_POWER_POLICIES_INFO_V2 nV_GPU_CLIENT_POWER_POLICIES_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 0, 2248);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
		*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
		if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo(hGpu, &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) == (_NvAPI_Status)0 && (byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) != 0)
		{
			int num = 0;
			int num2 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
			if (0 < num2)
			{
				long num3 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8));
				do
				{
					if (*(int*)num3 != (int)thermPolicyId)
					{
						num++;
						num3 += 348;
						continue;
					}
					long num4 = (long)num * 348L;
					float num5 = (float)(*(int*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 28))))) * 0.00390625f;
					float num6 = (float)(*(int*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 20))))) * 0.00390625f;
					float num7 = (float)(*(int*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 24))))) * 0.00390625f;
					if (!((double)fTemperature <= (double)num5) || !((double)fTemperature >= (double)num6))
					{
						break;
					}
					*(int*)(&nV_GPU_CLIENT_POWER_POLICIES_INFO_V) = 133320;
					if (global::_003CModule_003E.NvAPI_GPU_ClientPowerPoliciesGetInfo(hGpu, &nV_GPU_CLIENT_POWER_POLICIES_INFO_V) != 0 || System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 4)) == 0)
					{
						break;
					}
					int num8 = 0;
					int num9 = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 5));
					if (0 >= num9)
					{
						break;
					}
					long num10 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 8));
					do
					{
						if (*(int*)num10 != (int)pwrPolicyId)
						{
							num8++;
							num10 += 560;
							continue;
						}
						long num11 = (long)num8 * 560L;
						float num12 = *(uint*)((ref *(_003F*)num11) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 48))));
						float num13 = *(uint*)((ref *(_003F*)num11) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 24))));
						float num14 = *(uint*)((ref *(_003F*)num11) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLIENT_POWER_POLICIES_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_POWER_POLICIES_INFO_V, 36))));
						if ((double)fTemperature > (double)num7)
						{
							float num15 = (*pfPower = (float)(((double)fTemperature - (double)num7) / ((double)num5 - (double)num7) * ((double)num12 - (double)num14) + (double)num14));
							float num16 = ((!((double)num15 > (double)num12)) ? num15 : num12);
							*pfPower = num16 / 1000f;
						}
						else
						{
							double num17 = num14;
							float num18 = (*pfPower = (float)(num17 - ((double)num7 - (double)fTemperature) / ((double)num7 - (double)num6) * (num17 - (double)num13)));
							float num19 = ((!((double)num18 < (double)num13)) ? num18 : num13);
							*pfPower = num19 / 1000f;
						}
						result = true;
						break;
					}
					while (num8 < num9);
					break;
				}
				while (num < num2);
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getLinkedTempCap(uint nIndex, float fTemperature, ref float fPower)
	{
		//IL_0013: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out float num);
		if (getLinkedTempCap((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[nIndex], (NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID)1, (NV_GPU_CLIENT_POWER_POLICIES_POLICY_ID)0, fTemperature, &num))
		{
			fPower = num;
			return true;
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getFramerateLimit(ref uint ulValue)
	{
		//IL_0005: Expected I, but got I8
		//IL_0019: Expected I, but got I8
		//IL_0031: Expected I4, but got I8
		bool result = false;
		NvDRSSessionHandle__* ptr = null;
		if (global::_003CModule_003E.NvAPI_DRS_CreateSession(&ptr) == (_NvAPI_Status)0)
		{
			if (global::_003CModule_003E.NvAPI_DRS_LoadSettings(ptr) == (_NvAPI_Status)0)
			{
				NvDRSProfileHandle__* ptr2 = null;
				if (global::_003CModule_003E.NvAPI_DRS_GetBaseProfile(ptr, &ptr2) == (_NvAPI_Status)0)
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out _NVDRS_SETTING_V1 nVDRS_SETTING_V);
					// IL initblk instruction
					System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nVDRS_SETTING_V, 0, 12320);
					*(int*)(&nVDRS_SETTING_V) = 77856;
					if (global::_003CModule_003E.NvAPI_DRS_GetSetting(ptr, ptr2, 277041154u, &nVDRS_SETTING_V) == (_NvAPI_Status)0)
					{
						ulValue = System.Runtime.CompilerServices.Unsafe.As<_NVDRS_SETTING_V1, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nVDRS_SETTING_V, 8220));
					}
					result = true;
				}
			}
			global::_003CModule_003E.NvAPI_DRS_DestroySession(ptr);
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool setFramerateLimit(uint ulValue)
	{
		//IL_0005: Expected I, but got I8
		//IL_001c: Expected I, but got I8
		//IL_0046: Expected I4, but got I8
		bool flag = false;
		NvDRSSessionHandle__* ptr = null;
		if (global::_003CModule_003E.NvAPI_DRS_CreateSession(&ptr) == (_NvAPI_Status)0)
		{
			if (global::_003CModule_003E.NvAPI_DRS_LoadSettings(ptr) == (_NvAPI_Status)0)
			{
				NvDRSProfileHandle__* ptr2 = null;
				if (global::_003CModule_003E.NvAPI_DRS_GetBaseProfile(ptr, &ptr2) == (_NvAPI_Status)0)
				{
					_NvAPI_Status num;
					if (0 == ulValue)
					{
						num = global::_003CModule_003E.NvAPI_DRS_DeleteProfileSetting(ptr, ptr2, 277041154u);
					}
					else
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out _NVDRS_SETTING_V1 nVDRS_SETTING_V);
						// IL initblk instruction
						System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nVDRS_SETTING_V, 0, 12320);
						*(int*)(&nVDRS_SETTING_V) = 77856;
						System.Runtime.CompilerServices.Unsafe.As<_NVDRS_SETTING_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nVDRS_SETTING_V, 4100)) = 277041154;
						System.Runtime.CompilerServices.Unsafe.As<_NVDRS_SETTING_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nVDRS_SETTING_V, 4104)) = 0;
						System.Runtime.CompilerServices.Unsafe.As<_NVDRS_SETTING_V1, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nVDRS_SETTING_V, 8220)) = ulValue;
						num = global::_003CModule_003E.NvAPI_DRS_SetSetting(ptr, ptr2, &nVDRS_SETTING_V);
					}
					if (num == (_NvAPI_Status)0)
					{
						flag = global::_003CModule_003E.NvAPI_DRS_SaveSettings(ptr) == (_NvAPI_Status)0 || flag;
					}
				}
			}
			global::_003CModule_003E.NvAPI_DRS_DestroySession(ptr);
		}
		return flag;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool SetGpuLockFrequency(uint index, uint freqkHz)
	{
		//IL_0010: Expected I, but got I8
		return lockFrequency((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], (_NV_GPU_PUBLIC_CLOCK_ID)0, freqkHz);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool GetGpuLockFrequency(uint index, ref uint freqkHz)
	{
		//IL_0011: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		bool lockInfo = getLockInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], (_NV_PERF_CLIENT_LIMIT_ID)0, &num);
		if (lockInfo)
		{
			freqkHz = num;
		}
		return lockInfo;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool SetMemoryLockFrequency(uint index, uint freqkHz)
	{
		//IL_0010: Expected I, but got I8
		return lockFrequency((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], (_NV_GPU_PUBLIC_CLOCK_ID)4, freqkHz);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool GetMemoryLockFrequency(uint index, ref uint freqkHz)
	{
		//IL_0011: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		bool lockInfo = getLockInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], (_NV_PERF_CLIENT_LIMIT_ID)2, &num);
		if (lockInfo)
		{
			freqkHz = num;
		}
		return lockInfo;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool SetGpuLockVoltage(uint index, uint voltuV)
	{
		//IL_000e: Expected I4, but got I8
		//IL_0053: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_CLIENT_LIMITS_V1 nV_GPU_PERF_CLIENT_LIMITS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_CLIENT_LIMITS_V, 0, 780);
		*(int*)(&nV_GPU_PERF_CLIENT_LIMITS_V) = 131852;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 8)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 12)) = 6;
		if (voltuV == 0)
		{
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 20)) = 0;
		}
		else
		{
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 20)) = 3;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 28)) = voltuV;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 32)) = 0;
		}
		return global::_003CModule_003E.NvAPI_GPU_PerfClientLimitsSetStatus((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], &nV_GPU_PERF_CLIENT_LIMITS_V) == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool GetGpuLockVoltage(uint index, ref uint voltuV)
	{
		//IL_0011: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		bool lockInfo = getLockInfo((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], (_NV_PERF_CLIENT_LIMIT_ID)6, &num);
		if (lockInfo)
		{
			voltuV = num;
		}
		return lockInfo;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool StartManualScan(uint index)
	{
		//IL_000b: Expected I4, but got I8
		//IL_0022: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_START_OC_SCANNER_SETTINGS_V1 nV_GPU_CLIENT_START_OC_SCANNER_SETTINGS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_START_OC_SCANNER_SETTINGS_V, 0, 68);
		*(int*)(&nV_GPU_CLIENT_START_OC_SCANNER_SETTINGS_V) = 65604;
		return global::_003CModule_003E.NvAPI_GPU_ClientStartOcScanner((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], &nV_GPU_CLIENT_START_OC_SCANNER_SETTINGS_V) == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool StopManualScan(uint index)
	{
		//IL_000b: Expected I4, but got I8
		//IL_0022: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_STOP_OC_SCANNER_SETTINGS_V1 nV_GPU_CLIENT_STOP_OC_SCANNER_SETTINGS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_STOP_OC_SCANNER_SETTINGS_V, 0, 68);
		*(int*)(&nV_GPU_CLIENT_STOP_OC_SCANNER_SETTINGS_V) = 65604;
		return global::_003CModule_003E.NvAPI_GPU_ClientStopOcScanner((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], &nV_GPU_CLIENT_STOP_OC_SCANNER_SETTINGS_V) == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool GetLastScanResult(uint index)
	{
		//IL_000b: Expected I4, but got I8
		//IL_0022: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_GET_LAST_OC_SCANNER_RESULTS_V1 nV_GPU_CLIENT_GET_LAST_OC_SCANNER_RESULTS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_GET_LAST_OC_SCANNER_RESULTS_V, 0, 68);
		*(int*)(&nV_GPU_CLIENT_GET_LAST_OC_SCANNER_RESULTS_V) = 65604;
		return global::_003CModule_003E.NvAPI_GPU_ClientGetLastOcScannerResults((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], &nV_GPU_CLIENT_GET_LAST_OC_SCANNER_RESULTS_V) == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool RevertOc(uint index)
	{
		//IL_000b: Expected I4, but got I8
		//IL_0022: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_REVERT_OC_SETTINGS_V1 nV_GPU_CLIENT_REVERT_OC_SETTINGS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_REVERT_OC_SETTINGS_V, 0, 68);
		*(int*)(&nV_GPU_CLIENT_REVERT_OC_SETTINGS_V) = 65604;
		return global::_003CModule_003E.NvAPI_GPU_ClientRevertOc((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], &nV_GPU_CLIENT_REVERT_OC_SETTINGS_V) == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool getSystemType(uint nIndex, ref CLR_NV_SYSTEM_TYPE type)
	{
		//IL_0018: Expected I, but got I8
		NvPhysicalGpuHandle__*[] array = gpuHandles;
		if ((nuint)(int)nIndex >= (nuint)array.LongLength)
		{
			return false;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_SYSTEM_TYPE nV_SYSTEM_TYPE);
		if (global::_003CModule_003E.NvAPI_GPU_GetSystemType((NvPhysicalGpuHandle__*)((long[])(object)array)[nIndex], &nV_SYSTEM_TYPE) != 0)
		{
			return false;
		}
		type = (CLR_NV_SYSTEM_TYPE)nV_SYSTEM_TYPE;
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe bool GPU_GetAllTempsEx(uint index, uint mask, float[] sensors)
	{
		//IL_001b: Expected I4, but got I8
		//IL_0039: Expected I, but got I8
		bool result = false;
		delegate* unmanaged[Cdecl, Cdecl]<NvPhysicalGpuHandle__*, NV_GPU_THERMAL_EX_V2*, _NvAPI_Status> nvAPI_GPU_GetAllTempsEx = NvAPI_GPU_GetAllTempsEx;
		if (nvAPI_GPU_GetAllTempsEx != (delegate* unmanaged[Cdecl, Cdecl]<NvPhysicalGpuHandle__*, NV_GPU_THERMAL_EX_V2*, _NvAPI_Status>)null)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_THERMAL_EX_V2 nV_GPU_THERMAL_EX_V);
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_THERMAL_EX_V, 0, 168);
			*(int*)(&nV_GPU_THERMAL_EX_V) = 131240;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_THERMAL_EX_V2, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_THERMAL_EX_V, 4)) = mask;
			if (nvAPI_GPU_GetAllTempsEx((NvPhysicalGpuHandle__*)((long[])(object)gpuHandles)[index], &nV_GPU_THERMAL_EX_V) == (_NvAPI_Status)0)
			{
				int num = 0;
				long num2 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_THERMAL_EX_V, 40));
				do
				{
					if (((uint)(1 << num) & mask) != 0)
					{
						int num3 = *(int*)num2;
						if (num3 != 0)
						{
							sensors[num] = (float)((double)num3 * (1.0 / 256.0));
							result = true;
						}
					}
					num++;
					num2 += 4;
				}
				while (num < 32);
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getClocks(NvPhysicalGpuHandle__* hGpu, NV_GPU_CLOCK_FREQUENCIES_CLOCK_TYPE type, _NV_GPU_PUBLIC_CLOCK_ID clockId, uint* pulFrequency)
	{
		//IL_0010: Expected I4, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_CLOCK_FREQUENCIES_V2 nV_GPU_CLOCK_FREQUENCIES_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLOCK_FREQUENCIES_V, 0, 264);
		*(int*)(&nV_GPU_CLOCK_FREQUENCIES_V) = 196872;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLOCK_FREQUENCIES_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLOCK_FREQUENCIES_V, 4)) ^= (int)(((uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLOCK_FREQUENCIES_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLOCK_FREQUENCIES_V, 4)) ^ (uint)type) & 0xF);
		if (global::_003CModule_003E.NvAPI_GPU_GetAllClockFrequencies(hGpu, &nV_GPU_CLOCK_FREQUENCIES_V) == (_NvAPI_Status)0)
		{
			long num = (long)clockId * 8L;
			if ((*(int*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLOCK_FREQUENCIES_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLOCK_FREQUENCIES_V, 8)))) & 1) != 0)
			{
				*pulFrequency = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_CLOCK_FREQUENCIES_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLOCK_FREQUENCIES_V, 12))));
				result = true;
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getUsages(NvPhysicalGpuHandle__* hGpu, _NV_GPU_UTILIZATION_DOMAIN_ID domainId, uint* pulPercentage)
	{
		//IL_000d: Expected I4, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_DYNAMIC_PSTATES_INFO_EX nV_GPU_DYNAMIC_PSTATES_INFO_EX);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_DYNAMIC_PSTATES_INFO_EX, 0, 72);
		*(int*)(&nV_GPU_DYNAMIC_PSTATES_INFO_EX) = 65608;
		if (global::_003CModule_003E.NvAPI_GPU_GetDynamicPstatesInfoEx(hGpu, &nV_GPU_DYNAMIC_PSTATES_INFO_EX) == (_NvAPI_Status)0)
		{
			long num = (long)domainId * 8L;
			if ((*(int*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_DYNAMIC_PSTATES_INFO_EX, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_DYNAMIC_PSTATES_INFO_EX, 8)))) & 1) != 0)
			{
				*pulPercentage = *(uint*)((ref *(_003F*)num) + (ref System.Runtime.CompilerServices.Unsafe.As<NV_GPU_DYNAMIC_PSTATES_INFO_EX, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_DYNAMIC_PSTATES_INFO_EX, 12))));
				result = true;
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getPerfPolicyInfo(NvPhysicalGpuHandle__* hGpu, NV_GPU_PERF_POLICY_ID_SW policyId)
	{
		//IL_000d: Expected I4, but got I8
		bool flag = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_POLICIES_INFO_PARAMS_V1 nV_GPU_PERF_POLICIES_INFO_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 0, 76);
		*(int*)(&nV_GPU_PERF_POLICIES_INFO_PARAMS_V) = 65612;
		if (global::_003CModule_003E.NvAPI_GPU_PerfPoliciesGetInfo(hGpu, &nV_GPU_PERF_POLICIES_INFO_PARAMS_V) == (_NvAPI_Status)0)
		{
			flag = ((1 << (int)policyId) & System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_POLICIES_INFO_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 8))) != 0 || flag;
		}
		return flag;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getPerfPolicyStatus(NvPhysicalGpuHandle__* hGpu, NV_GPU_PERF_POLICY_ID_SW policyId, uint* ulLimit)
	{
		//IL_000d: Expected I4, but got I8
		//IL_003a: Expected I4, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_POLICIES_INFO_PARAMS_V1 nV_GPU_PERF_POLICIES_INFO_PARAMS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 0, 76);
		*(int*)(&nV_GPU_PERF_POLICIES_INFO_PARAMS_V) = 65612;
		if (global::_003CModule_003E.NvAPI_GPU_PerfPoliciesGetInfo(hGpu, &nV_GPU_PERF_POLICIES_INFO_PARAMS_V) == (_NvAPI_Status)0)
		{
			int num = 1 << (int)policyId;
			if ((num & System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_POLICIES_INFO_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_POLICIES_INFO_PARAMS_V, 8))) != 0)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_POLICIES_STATUS_PARAMS_V1 nV_GPU_PERF_POLICIES_STATUS_PARAMS_V);
				// IL initblk instruction
				System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_POLICIES_STATUS_PARAMS_V, 0, 1360);
				*(int*)(&nV_GPU_PERF_POLICIES_STATUS_PARAMS_V) = 66896;
				System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_POLICIES_STATUS_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_POLICIES_STATUS_PARAMS_V, 4)) = num;
				if (global::_003CModule_003E.NvAPI_GPU_PerfPoliciesGetStatus(hGpu, &nV_GPU_PERF_POLICIES_STATUS_PARAMS_V) == (_NvAPI_Status)0)
				{
					*ulLimit = (((System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_POLICIES_STATUS_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_POLICIES_STATUS_PARAMS_V, 16)) & System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_POLICIES_STATUS_PARAMS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_POLICIES_STATUS_PARAMS_V, 4))) != 0) ? 1u : 0u);
					result = true;
				}
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getClockOffset(NvPhysicalGpuHandle__* hGpu, _NV_GPU_PUBLIC_CLOCK_ID clockId, int* pnDelta, int* pnMinDelta, int* pnMaxDelta)
	{
		//IL_000e: Expected I4, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_PERF_PSTATES20_INFO_V2 nV_GPU_PERF_PSTATES20_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_PSTATES20_INFO_V, 0, 7416);
		*(int*)(&nV_GPU_PERF_PSTATES20_INFO_V) = 204024;
		if (global::_003CModule_003E.NvAPI_GPU_GetPstates20(hGpu, &nV_GPU_PERF_PSTATES20_INFO_V) == (_NvAPI_Status)0)
		{
			uint num = 0u;
			if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 8)))
			{
				do
				{
					long num2 = (long)num * 456L;
					if (*(int*)((ref *(_003F*)num2) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 20)))) == 0)
					{
						uint num3 = 0u;
						if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 12)))
						{
							do
							{
								if (*(int*)((ref *(_003F*)((long)num3 * 44L + num2)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 28)))) != (int)clockId)
								{
									num3++;
									continue;
								}
								long num4 = (long)num * 456L + (long)num3 * 44L;
								*pnDelta = *(int*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 40))));
								*pnMinDelta = *(int*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 44))));
								*pnMaxDelta = *(int*)((ref *(_003F*)num4) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 48))));
								return true;
							}
							while (num3 < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 12)));
						}
					}
					num++;
				}
				while (num < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 8)));
			}
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool setClockOffset(NvPhysicalGpuHandle__* hGpu, _NV_GPU_PUBLIC_CLOCK_ID clockId, int nDelta)
	{
		//IL_000e: Expected I4, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_PERF_PSTATES20_INFO_V2 nV_GPU_PERF_PSTATES20_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_PSTATES20_INFO_V, 0, 7416);
		*(int*)(&nV_GPU_PERF_PSTATES20_INFO_V) = 204024;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 8)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 12)) = 1;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 16)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 20)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, _NV_GPU_PUBLIC_CLOCK_ID>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 28)) = clockId;
		System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_PERF_PSTATES20_INFO_V2, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_PSTATES20_INFO_V, 40)) = nDelta;
		_NvAPI_Status nvAPI_Status = global::_003CModule_003E.NvAPI_GPU_SetPstates20(hGpu, &nV_GPU_PERF_PSTATES20_INFO_V);
		int num;
		int num2;
		if (nvAPI_Status == (_NvAPI_Status)0)
		{
			num = 1;
		}
		else
		{
			num = 0;
			if (nvAPI_Status == (_NvAPI_Status)(-217))
			{
				num2 = 1;
				goto IL_005f;
			}
		}
		num2 = 0;
		goto IL_005f;
		IL_005f:
		isGpuInDebugMode = (byte)num2 != 0;
		return (num != 0) ? true : false;
	}

	private unsafe _NvAPI_Status clientClkInfoGet(NvPhysicalGpuHandle__* hGpu, ManagedNvApi.CLIENT_CLK_INFO* pClientClkInfo)
	{
		//IL_001d: Expected I, but got I8
		*(int*)pClientClkInfo = 67880;
		_NvAPI_Status nvAPI_Status = global::_003CModule_003E.NvAPI_GPU_ClockClientClkDomainsGetInfo(hGpu, (_NV_GPU_CLOCK_CLIENT_CLK_DOMAINS_INFO_V1*)pClientClkInfo);
		if (nvAPI_Status != 0)
		{
			return nvAPI_Status;
		}
		ManagedNvApi.CLIENT_CLK_INFO* ptr = (ManagedNvApi.CLIENT_CLK_INFO*)((ulong)(nint)pClientClkInfo + 2344uL);
		*(int*)ptr = 71724;
		return global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetInfo(hGpu, (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_INFO_V1*)ptr);
	}

	private unsafe _NvAPI_Status clientClkVfPointsGetStatus(NvPhysicalGpuHandle__* hGpu, CLIENT_CLK_VF_POINTS_STATUS* pClientClkVfPointsStatus)
	{
		*(int*)pClientClkVfPointsStatus = 219916;
		_NvAPI_Status nvAPI_Status = global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetStatus(hGpu, (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_STATUS_V3*)pClientClkVfPointsStatus);
		if ((_NvAPI_Status)(-9) == nvAPI_Status)
		{
			*(int*)pClientClkVfPointsStatus = 138280;
			nvAPI_Status = global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetStatus(hGpu, (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_STATUS_V3*)pClientClkVfPointsStatus);
			if ((_NvAPI_Status)(-9) == nvAPI_Status)
			{
				*(int*)pClientClkVfPointsStatus = 72744;
				nvAPI_Status = global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetStatus(hGpu, (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_STATUS_V3*)pClientClkVfPointsStatus);
			}
		}
		return nvAPI_Status;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getGraphicsVfMaxkHz(NvPhysicalGpuHandle__* hGpu, uint* pFreqkHz)
	{
		if (pFreqkHz != null)
		{
			*pFreqkHz = 0u;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out ManagedNvApi.CLIENT_CLK_INFO cLIENT_CLK_INFO);
			*(int*)(&cLIENT_CLK_INFO) = 67880;
			if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkDomainsGetInfo(hGpu, (_NV_GPU_CLOCK_CLIENT_CLK_DOMAINS_INFO_V1*)(&cLIENT_CLK_INFO)) == (_NvAPI_Status)0)
			{
				System.Runtime.CompilerServices.Unsafe.As<ManagedNvApi.CLIENT_CLK_INFO, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2344)) = 71724;
				if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetInfo(hGpu, (_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_INFO_V1*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2344))) == (_NvAPI_Status)0)
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_STATUS_V3 nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_STATUS_V);
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_STATUS_V, 4), ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 2348), 32);
					*(int*)(&nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_STATUS_V) = 219916;
					if (global::_003CModule_003E.NvAPI_GPU_ClockClientClkVfPointsGetStatus(hGpu, &nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_STATUS_V) != 0)
					{
						return false;
					}
					uint num = 0u;
					long num2 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 40));
					do
					{
						if (0 == ((1 << (int)num) & *(int*)((ref *(_003F*)(((ulong)num >> 5) * 4)) + (ref System.Runtime.CompilerServices.Unsafe.As<ManagedNvApi.CLIENT_CLK_INFO, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cLIENT_CLK_INFO, 4))))) || 0 != *(int*)(num2 + 4))
						{
							num++;
							num2 += 72;
							continue;
						}
						*pFreqkHz = *(uint*)((ref *(_003F*)((long)(*(byte*)(num2 + 49)) * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLOCK_CLIENT_CLK_VF_POINTS_STATUS_V, 108))));
						return true;
					}
					while (num < 32);
					return false;
				}
			}
			return false;
		}
		return false;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getTdpLimit(NvPhysicalGpuHandle__* hGpu, NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID policyId, int* bRemoveTdpLimit)
	{
		//IL_0011: Expected I4, but got I8
		//IL_001f: Expected I4, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3 nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 0, 1352);
		*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
		if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo(hGpu, &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) == (_NvAPI_Status)0 && (byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) != 0)
		{
			*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) = 197960;
			System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)) = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
			uint num = 0u;
			if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5)))
			{
				do
				{
					long num2 = num;
					*(int*)((ref *(_003F*)(num2 * 336)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8)))) = *(int*)((ref *(_003F*)(num2 * 348)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8))));
					num++;
				}
				while (num < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)));
			}
			if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetStatus(hGpu, &nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) == (_NvAPI_Status)0)
			{
				uint num3 = 0u;
				uint num4 = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4));
				if (0 < num4)
				{
					do
					{
						if (*(int*)((ref *(_003F*)((long)num3 * 336L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8)))) != (int)policyId)
						{
							num3++;
							continue;
						}
						*bRemoveTdpLimit = *(byte*)((ref *(_003F*)((long)num3 * 336L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 20)))) & 1;
						result = true;
						break;
					}
					while (num3 < num4);
				}
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool setTdpLimit(NvPhysicalGpuHandle__* hGpu, NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID policyId, int bRemoveTdpLimit)
	{
		//IL_0010: Expected I4, but got I8
		//IL_001e: Expected I4, but got I8
		bool result = false;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3 nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 0, 1400);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3 nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 0, 1352);
		*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) = 198008;
		if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetInfo(hGpu, &nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V) == (_NvAPI_Status)0 && (byte)(System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 4)) & 1) != 0)
		{
			uint num = 0u;
			if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5)))
			{
				do
				{
					if (*(int*)((ref *(_003F*)((long)num * 348L)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8)))) == (int)policyId)
					{
						*(int*)(&nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) = 197960;
						System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)) = System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5));
						num = 0u;
						if (0u < (uint)System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5)))
						{
							do
							{
								long num2 = num;
								*(int*)((ref *(_003F*)(num2 * 336)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8)))) = *(int*)((ref *(_003F*)(num2 * 348)) + (ref System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 8))));
								num++;
							}
							while (num < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)));
						}
						if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesGetStatus(hGpu, &nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) == (_NvAPI_Status)0)
						{
							System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 4)) = 1;
							System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, NV_GPU_CLIENT_THERMAL_POLICIES_POLICY_ID>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 8)) = policyId;
							System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 20)) = (sbyte)(((System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 20)) ^ bRemoveTdpLimit) & 1) ^ System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V, 20)));
							if (global::_003CModule_003E.NvAPI_GPU_ClientThermalPoliciesSetStatus(hGpu, &nV_GPU_CLIENT_THERMAL_POLICIES_STATUS_V) == (_NvAPI_Status)0)
							{
								result = true;
								break;
							}
						}
					}
					num++;
				}
				while (num < System.Runtime.CompilerServices.Unsafe.As<_NV_GPU_CLIENT_THERMAL_POLICIES_INFO_V3, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_CLIENT_THERMAL_POLICIES_INFO_V, 5)));
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool lockFrequency(NvPhysicalGpuHandle__* hGpu, _NV_GPU_PUBLIC_CLOCK_ID clkId, uint freqkHz)
	{
		//IL_000e: Expected I4, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_CLIENT_LIMITS_V1 nV_GPU_PERF_CLIENT_LIMITS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_CLIENT_LIMITS_V, 0, 780);
		*(int*)(&nV_GPU_PERF_CLIENT_LIMITS_V) = 131852;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 8)) = 2;
		if (clkId == (_NV_GPU_PUBLIC_CLOCK_ID)0)
		{
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 12)) = 0;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 36)) = 1;
		}
		else
		{
			if (clkId != (_NV_GPU_PUBLIC_CLOCK_ID)4)
			{
				return false;
			}
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 12)) = 2;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 36)) = 3;
		}
		if (freqkHz == 0)
		{
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 20)) = 0;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 44)) = 0;
		}
		else
		{
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 20)) = 2;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 28)) = freqkHz;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, _NV_GPU_PUBLIC_CLOCK_ID>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 32)) = clkId;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 44)) = 2;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 52)) = freqkHz;
			System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, _NV_GPU_PUBLIC_CLOCK_ID>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 56)) = clkId;
		}
		return global::_003CModule_003E.NvAPI_GPU_PerfClientLimitsSetStatus(hGpu, &nV_GPU_PERF_CLIENT_LIMITS_V) == (_NvAPI_Status)0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool getLockInfo(NvPhysicalGpuHandle__* hGpu, _NV_PERF_CLIENT_LIMIT_ID limitId, uint* pfreqkHz)
	{
		//IL_000e: Expected I4, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NV_GPU_PERF_CLIENT_LIMITS_V1 nV_GPU_PERF_CLIENT_LIMITS_V);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref nV_GPU_PERF_CLIENT_LIMITS_V, 0, 780);
		bool result = false;
		*(int*)(&nV_GPU_PERF_CLIENT_LIMITS_V) = 131852;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, _NV_PERF_CLIENT_LIMIT_ID>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 12)) = limitId;
		System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 8)) = 1;
		if (global::_003CModule_003E.NvAPI_GPU_PerfClientLimitsGetStatus(hGpu, &nV_GPU_PERF_CLIENT_LIMITS_V) == (_NvAPI_Status)0 && 0u < (uint)System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 8)))
		{
			long num = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 28));
			long num2 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 20));
			long num3 = (nint)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 12));
			uint num4 = System.Runtime.CompilerServices.Unsafe.As<NV_GPU_PERF_CLIENT_LIMITS_V1, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref nV_GPU_PERF_CLIENT_LIMITS_V, 8));
			do
			{
				switch (*(int*)num3)
				{
				case 6:
					if (*(int*)num2 == 0)
					{
						*pfreqkHz = 0u;
					}
					else
					{
						*pfreqkHz = *(uint*)num;
					}
					break;
				case 0:
				case 2:
					if (*(int*)num2 == 0)
					{
						*pfreqkHz = 0u;
					}
					else
					{
						*pfreqkHz = *(uint*)num;
					}
					result = true;
					break;
				}
				num3 += 24;
				num2 += 24;
				num += 24;
				num4 += uint.MaxValue;
			}
			while (num4 != 0);
		}
		return result;
	}

	private void CheckRGBW(ref byte red, ref byte green, ref byte blue, ref byte white)
	{
		byte b = byte.MaxValue;
		byte b2 = 0;
		byte b3 = red;
		b = (((uint)b3 < 255u) ? b3 : b);
		byte b4 = green;
		b = (((uint)b4 < (uint)b) ? b4 : b);
		byte b5 = blue;
		b = (((uint)b5 < (uint)b) ? b5 : b);
		b2 = ((b3 != 0) ? b3 : b2);
		b2 = (((uint)b4 > (uint)b2) ? b4 : b2);
		b2 = (((uint)b5 > (uint)b2) ? b5 : b2);
		if (b2 - b <= 10)
		{
			white = (byte)((b2 + b) / 2);
			red = 0;
			green = 0;
			blue = 0;
		}
		else
		{
			white = 0;
		}
	}

	public unsafe NvApi()
	{
		//IL_000f: Expected I, but got I8
		//IL_0084: Expected I, but got I8
		//IL_008a: Expected I, but got I8
		//IL_009e: Expected I8, but got I
		//IL_00a9: Expected I8, but got I
		if (global::_003CModule_003E.NvAPI_Initialize() != 0)
		{
			return;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0EA_0040PEAUNvPhysicalGpuHandle___0040_0040 _0024ArrayType_0024_0024_0024BY0EA_0040PEAUNvPhysicalGpuHandle___0040_0040);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		if (global::_003CModule_003E.NvAPI_EnumPhysicalGPUs((NvPhysicalGpuHandle__**)(&_0024ArrayType_0024_0024_0024BY0EA_0040PEAUNvPhysicalGpuHandle___0040_0040), &num) == (_NvAPI_Status)0)
		{
			gpuCount = num;
			NvPhysicalGpuHandle__*[] array = (gpuHandles = new NvPhysicalGpuHandle__*[num]);
			uint num2 = 0u;
			if (0 < num)
			{
				do
				{
					ref NvPhysicalGpuHandle__* reference = ref array[num2];
					System.Runtime.CompilerServices.Unsafe.As<NvPhysicalGpuHandle__*, long>(ref reference) = *(long*)((ref *(_003F*)((long)num2 * 8L)) + (ref *(_003F*)(&_0024ArrayType_0024_0024_0024BY0EA_0040PEAUNvPhysicalGpuHandle___0040_0040)));
					num2++;
				}
				while (num2 < num);
			}
			uint num3 = 0u;
			uint num4 = num - 1;
			if (0 < num4)
			{
				do
				{
					uint num5 = num4;
					if (num5 > num3)
					{
						uint num6 = num5 - 1;
						do
						{
							NvPhysicalGpuHandle__* ptr = (NvPhysicalGpuHandle__*)((long[])(object)array)[num5];
							NvPhysicalGpuHandle__* ptr2 = (NvPhysicalGpuHandle__*)((long[])(object)array)[num6];
							if (ptr < ptr2)
							{
								NvPhysicalGpuHandle__* ptr3 = ptr;
								System.Runtime.CompilerServices.Unsafe.As<NvPhysicalGpuHandle__*, long>(ref array[num5]) = (nint)ptr2;
								System.Runtime.CompilerServices.Unsafe.As<NvPhysicalGpuHandle__*, long>(ref array[num6]) = (nint)ptr3;
							}
							num5 += uint.MaxValue;
							num6 += uint.MaxValue;
						}
						while (num5 > num3);
					}
					num3++;
					num4 = num - 1;
				}
				while (num3 < num4);
			}
			isInitialized = true;
			HINSTANCE__* ptr4 = global::_003CModule_003E.LoadLibraryA((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0M_0040EEJEDLDA_0040nvapi64_003F4dll_0040));
			if (ptr4 != null)
			{
				delegate* unmanaged[Cdecl, Cdecl]<uint, int*> procAddress = (delegate* unmanaged[Cdecl, Cdecl]<uint, int*>)global::_003CModule_003E.GetProcAddress(ptr4, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref global::_003CModule_003E._003F_003F_C_0040_0BF_0040OFPFEMKN_0040nvapi_QueryInterface_0040));
				if (procAddress != (delegate* unmanaged[Cdecl, Cdecl]<uint, int*>)null)
				{
					NvAPI_GPU_GetAllTempsEx = (delegate* unmanaged[Cdecl, Cdecl]<NvPhysicalGpuHandle__*, NV_GPU_THERMAL_EX_V2*, _NvAPI_Status>)procAddress(1711159981u);
				}
				global::_003CModule_003E.FreeLibrary(ptr4);
			}
		}
		powerTargetArray = new float[num];
	}

	private void _007ENvApi()
	{
		global::_003CModule_003E.NvAPI_Unload();
	}

	public NVAPI_STATUS Initialize()
	{
		_NvAPI_Status nvAPI_Status = global::_003CModule_003E.NvAPI_Initialize();
		int num = ((nvAPI_Status == (_NvAPI_Status)0) ? 1 : 0);
		isInitialized = (byte)num != 0;
		return (NVAPI_STATUS)nvAPI_Status;
	}

	public unsafe IntPtr getGpuHandle(uint nIndex)
	{
		//IL_001e: Expected I, but got I8
		//IL_0016: Expected I, but got I8
		if (nIndex < gpuCount)
		{
			return (IntPtr)(void*)((long[])(object)gpuHandles)[nIndex];
		}
		return (IntPtr)(void*)null;
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
	{
		if (A_0)
		{
			global::_003CModule_003E.NvAPI_Unload();
		}
		else
		{
			base.Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(A_0: true);
		GC.SuppressFinalize(this);
	}
}
