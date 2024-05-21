mergeInto(LibraryManager.library, {
StartExperiment: function (str) {
    ReactUnityWebGL.StartExperiment(Pointer_stringify(str))
  },

SendCode: function (str) {
    ReactUnityWebGL.ReceiveCode(Pointer_stringify(str))
  },

SendCodeName: function (str) {
    ReactUnityWebGL.ReceiveCodeName(Pointer_stringify(str))
  }
})