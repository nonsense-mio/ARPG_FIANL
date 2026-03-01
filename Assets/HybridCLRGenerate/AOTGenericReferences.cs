using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"GameFramework.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// }}

	public void RefMethods()
	{
		// System.Void Framework.IArchitecture.SendEvent<ARPG.LaunchGameEvent>(ARPG.LaunchGameEvent)
	}
}