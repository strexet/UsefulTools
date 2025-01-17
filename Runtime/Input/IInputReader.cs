namespace StrexetGames.UsefulTools.Runtime.Input
{
	/// <summary>
	/// Input reader interface for Unity Input package.
	/// It is convenient to inherit from this interface and ScriptableObject to create a custom input reader.
	/// Create an inputActions field to store input actions in the custom input reader implementation.
	/// Implement creation and enable of input actions in EnablePlayerActions method.
	/// </summary>
	public interface IInputReader
	{
		/// <summary>
		/// Default implementation of this method would be:
		/// 
		/// if (inputActions == null) {
		///		inputActions = new SomeInputActions();
		///		inputActions.SomeInputReceiver.SetCallbacks(this);
		/// }
		/// inputActions.Enable();
		/// </summary>
		void EnablePlayerActions();
	}
}