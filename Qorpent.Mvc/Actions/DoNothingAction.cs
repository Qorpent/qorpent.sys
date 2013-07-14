namespace Qorpent.Mvc.Actions {
    [Action("_sys.donothing", Role = "DEFAULT", Help = "Do absolutely nothing")]
    class DoNothingAction : ActionBase {
        protected override object MainProcess() {
            return "done.";
        }
    }
}
