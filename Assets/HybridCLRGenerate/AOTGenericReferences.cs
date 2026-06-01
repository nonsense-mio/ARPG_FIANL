using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"System.Core.dll",
		"System.dll",
		"Unity.Animation.Rigging.dll",
		"Unity.InputSystem.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.JSONSerializeModule.dll",
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// System.Action<ARPG.AimActionEvent>
	// System.Action<ARPG.BombHitEvent>
	// System.Action<ARPG.BossHudChangedEvent>
	// System.Action<ARPG.BossPhaseShiftEvent>
	// System.Action<ARPG.CharacterDamageEvent>
	// System.Action<ARPG.CharacterDeathEvent>
	// System.Action<ARPG.EnableInputEvent>
	// System.Action<ARPG.FireBallHitEvent>
	// System.Action<ARPG.GameDataLoadedEvent>
	// System.Action<ARPG.InteractPromptEvent>
	// System.Action<ARPG.NPCFollowPlayerEvent>
	// System.Action<ARPG.PerformInteractionEvent>
	// System.Action<ARPG.PlayerCastSpellEvent>
	// System.Action<ARPG.PlayerDrinkPotionEvent>
	// System.Action<ARPG.PlayerPosionEvent>
	// System.Action<ARPG.PlayerWarmUpSpellEvent>
	// System.Action<ARPG.SelectWindowEvent>
	// System.Action<ARPG.SlashEvent>
	// System.Action<ARPG.TaskCompletedEvent>
	// System.Action<ARPG.TaskStartedEvent>
	// System.Action<ARPG.TaskTurnedInEvent>
	// System.Action<LitJson.PropertyMetadata>
	// System.Action<SpawnBossEvent>
	// System.Action<SpawnEnemyEvent>
	// System.Action<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Action<UnityEngine.InputSystem.InputAction.CallbackContext>
	// System.Action<byte>
	// System.Action<float>
	// System.Action<int>
	// System.Action<object,object,object>
	// System.Action<object,object>
	// System.Action<object>
	// System.Collections.Generic.ArraySortHelper<LitJson.PropertyMetadata>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<LitJson.PropertyMetadata>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary.Enumerator<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary.Enumerator<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary.Enumerator<object,byte>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,byte>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary.KeyCollection<object,byte>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,byte>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary.ValueCollection<object,byte>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.Dictionary<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.Dictionary<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.Dictionary<object,byte>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<LitJson.ArrayMetadata>
	// System.Collections.Generic.EqualityComparer<LitJson.ObjectMetadata>
	// System.Collections.Generic.EqualityComparer<LitJson.PropertyMetadata>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<LitJson.PropertyMetadata>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,LitJson.ArrayMetadata>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,LitJson.ObjectMetadata>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,LitJson.PropertyMetadata>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,byte>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<LitJson.PropertyMetadata>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<int,object>
	// System.Collections.Generic.IDictionary<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<LitJson.PropertyMetadata>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,LitJson.ArrayMetadata>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,LitJson.ObjectMetadata>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,LitJson.PropertyMetadata>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,byte>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<LitJson.PropertyMetadata>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,LitJson.ArrayMetadata>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,LitJson.ObjectMetadata>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,LitJson.PropertyMetadata>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,byte>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<LitJson.PropertyMetadata>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.KeyValuePair<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.KeyValuePair<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.KeyValuePair<object,byte>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<LitJson.PropertyMetadata>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<LitJson.PropertyMetadata>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<LitJson.PropertyMetadata>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<LitJson.ArrayMetadata>
	// System.Collections.Generic.ObjectEqualityComparer<LitJson.ObjectMetadata>
	// System.Collections.Generic.ObjectEqualityComparer<LitJson.PropertyMetadata>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.Stack.Enumerator<int>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<int>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.Collection<int>
	// System.Collections.ObjectModel.Collection<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<LitJson.PropertyMetadata>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<LitJson.PropertyMetadata>
	// System.Comparison<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Converter<object,int>
	// System.Func<object,byte>
	// System.Func<object,object>
	// System.IEquatable<object>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Nullable<ARPG.BossHudData>
	// System.Nullable<int>
	// System.Predicate<LitJson.PropertyMetadata>
	// System.Predicate<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Predicate<int>
	// System.Predicate<object>
	// UnityEngine.Animations.Rigging.AnimationJobBinder<UnityEngine.Animations.Rigging.TwoBoneIKConstraintJob,UnityEngine.Animations.Rigging.TwoBoneIKConstraintData>
	// UnityEngine.Animations.Rigging.RigConstraint<UnityEngine.Animations.Rigging.TwoBoneIKConstraintJob,UnityEngine.Animations.Rigging.TwoBoneIKConstraintData,object>
	// UnityEngine.Events.InvokableCall<byte>
	// UnityEngine.Events.InvokableCall<float>
	// UnityEngine.Events.InvokableCall<object>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityAction<float>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityEvent<byte>
	// UnityEngine.Events.UnityEvent<float>
	// UnityEngine.Events.UnityEvent<object>
	// UnityEngine.InputSystem.InputBindingComposite<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputProcessor<UnityEngine.Vector2>
	// UnityEngine.InputSystem.Utilities.InlinedArray<object>
	// }}

	public void RefMethods()
	{
		// object System.Activator.CreateInstance<object>()
		// System.Collections.Generic.List<int> System.Collections.Generic.List<object>.ConvertAll<int>(System.Converter<object,int>)
		// int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<UnityEngine.Vector2>(UnityEngine.Vector2&)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<UnityEngine.Vector2>()
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.Component.GetComponentInParent<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// object UnityEngine.GameObject.GetComponentInParent<object>()
		// object UnityEngine.GameObject.GetComponentInParent<object>(bool)
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputAction.CallbackContext.ReadValue<UnityEngine.Vector2>()
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ApplyProcessors<UnityEngine.Vector2>(int,UnityEngine.Vector2,UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>)
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ReadValue<UnityEngine.Vector2>(int,int,bool)
		// object UnityEngine.JsonUtility.FromJson<object>(string)
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object[] UnityEngine.Object.FindObjectsOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion)
		// object[] UnityEngine.Resources.ConvertObjects<object>(UnityEngine.Object[])
		// object YooAsset.AssetHandle.GetAssetObject<object>()
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string,uint)
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetSync<object>(string)
	}
}