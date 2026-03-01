using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"GameFramework.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// System.Action<ARPG.ActivateBossFightRequestEvent>
	// System.Action<ARPG.BossDefeatedRequestEvent>
	// System.Action<object>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<object>
	// System.Nullable<ARPG.BossHudData>
	// System.Predicate<object>
	// }}

	public void RefMethods()
	{
		// System.Void Framework.CanSendEventExtension.SendEvent<ARPG.BossHudChangedEvent>(Framework.ICanSendEvent,ARPG.BossHudChangedEvent)
		// Framework.IUnRegister Framework.IArchitecture.RegisterEvent<ARPG.ActivateBossFightRequestEvent>(System.Action<ARPG.ActivateBossFightRequestEvent>)
		// Framework.IUnRegister Framework.IArchitecture.RegisterEvent<ARPG.BossDefeatedRequestEvent>(System.Action<ARPG.BossDefeatedRequestEvent>)
		// System.Void Framework.IArchitecture.SendCommand<object>(object)
		// System.Void Framework.IArchitecture.SendEvent<ARPG.BossHudChangedEvent>(ARPG.BossHudChangedEvent)
		// System.Void Framework.IArchitecture.SendEvent<ARPG.LaunchGameEvent>(ARPG.LaunchGameEvent)
	}
}