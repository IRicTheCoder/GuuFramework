namespace Guu
{
    /// <summary>Marks the loading state of the </summary>
    public enum LoadingState
    {
        /// <summary>Means no loading state is running yet</summary>
        NONE = 0,
        /// <summary>The initialization loading state. When the method <see cref="IModLoad.Init"/> runs</summary>
        INIT = 1,
        /// <summary>The pre-load loading state. When the method <see cref="IModLoad.PreLoad"/> runs</summary>
        PRE_LOAD = 2,
        /// <summary>The registration loading state. When the method <see cref="IModLoad.Register"/> runs</summary>
        REGISTER = 3,
        /// <summary>The load loading state. When the method <see cref="IModLoad.Load"/> runs</summary>
        LOAD = 4,
        /// <summary>The handle loading state. When the method <see cref="IModLoad.Handle"/> runs</summary>
        HANDLE = 5,
        /// <summary>The post-load loading state. When the method <see cref="IModLoad.PostLoad"/> runs</summary>
        POST_LOAD = 6,
        /// <summary>The comms loading state. When the method <see cref="IModLoad.Comms"/> runs</summary>
        COMMS = 7,
        /// <summary>Means the entire loading process is complete</summary>
        DONE = 8
    }
}