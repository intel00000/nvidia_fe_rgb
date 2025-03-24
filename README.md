Just a proved of concept
Using EVGA Precision X1 ManagedNvApi.dll to control the NVIDIA FE card lighting.

A more rigorous approach should be using the official NvAPI (https://github.com/NVIDIA/nvapi/blob/main/nvapi.h), which contain similar method for controlling illumination zones.

NvAPI_GPU_ClientIllumZonesGetInfo
NvAPI_GPU_ClientIllumZonesGetControl
NvAPI_GPU_ClientIllumZonesSetControl
