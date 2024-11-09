using System;
using System.IO;
using System.Text;
using SD = System.Drawing;

using Rhino;
using Grasshopper.Kernel;

namespace RhinoCodePlatform.Rhino3D.Projects.Plugin.GH
{
  public sealed class AssemblyInfo : GH_AssemblyInfo
  {
    static readonly string s_assemblyIconData = "[[ASSEMBLY-ICON]]";
    static readonly string s_categoryIconData = "[[ASSEMBLY-CATEGORY-ICON]]";

    public static readonly SD.Bitmap PluginIcon = default;
    public static readonly SD.Bitmap PluginCategoryIcon = default;

    static AssemblyInfo()
    {
      if (!s_assemblyIconData.Contains("ASSEMBLY-ICON"))
      {
        using (var aicon = new MemoryStream(Convert.FromBase64String(s_assemblyIconData)))
          PluginIcon = new SD.Bitmap(aicon);
      }

      if (!s_categoryIconData.Contains("ASSEMBLY-CATEGORY-ICON"))
      {
        using (var cicon = new MemoryStream(Convert.FromBase64String(s_categoryIconData)))
          PluginCategoryIcon = new SD.Bitmap(cicon);
      }
    }

    public override Guid Id { get; } = new Guid("4e8b8bae-c8af-42f2-8db7-7fe733dc795d");

    public override string AssemblyName { get; } = "Lattic3.Components";
    public override string AssemblyVersion { get; } = "0.1.1.9058";
    public override string AssemblyDescription { get; } = "The Lattic3 plugin is intended to design a new generation of diagrid-based lattice elements. The elements are designed from the standard circular hollow steel sections to maintain their same inertia with a reduction of material use.  ";
    public override string AuthorName { get; } = "Marco Palma";
    public override string AuthorContact { get; } = "marco.palma@tuwien.ac.at";
    public override GH_LibraryLicense AssemblyLicense { get; } = GH_LibraryLicense.unset;
    public override SD.Bitmap AssemblyIcon { get; } = PluginIcon;
  }

  public class ProjectComponentPlugin : GH_AssemblyPriority
  {
    static readonly Guid s_projectId = new Guid("4e8b8bae-c8af-42f2-8db7-7fe733dc795d");
    static readonly string s_projectData = "ew0KICAiaG9zdCI6IHsNCiAgICAibmFtZSI6ICJSaGlubzNEIiwNCiAgICAidmVyc2lvbiI6ICI4LjEzLjI0Mjg5XHUwMDJCMTMwMDEiLA0KICAgICJvcyI6ICJ3aW5kb3dzIiwNCiAgICAiYXJjaCI6ICJ4NjQiDQogIH0sDQogICJpZCI6ICI0ZThiOGJhZS1jOGFmLTQyZjItOGRiNy03ZmU3MzNkYzc5NWQiLA0KICAiaWRlbnRpdHkiOiB7DQogICAgIm5hbWUiOiAiTGF0dGljMyIsDQogICAgInZlcnNpb24iOiAiMC4xLjEiLA0KICAgICJwdWJsaXNoZXIiOiB7DQogICAgICAiZW1haWwiOiAibWFyY28ucGFsbWFAdHV3aWVuLmFjLmF0IiwNCiAgICAgICJuYW1lIjogIk1hcmNvIFBhbG1hIiwNCiAgICAgICJjb21wYW55IjogIlRVIFdpZW4gLSBJbnN0aXR1dGUgb2YgQXJ0IGFuZCBEZXNpZ24iDQogICAgfSwNCiAgICAiZGVzY3JpcHRpb24iOiAiVGhlIExhdHRpYzMgcGx1Z2luIGlzIGludGVuZGVkIHRvIGRlc2lnbiBhIG5ldyBnZW5lcmF0aW9uIG9mIGRpYWdyaWQtYmFzZWQgbGF0dGljZSBlbGVtZW50cy4gVGhlIGVsZW1lbnRzIGFyZSBkZXNpZ25lZCBmcm9tIHRoZSBzdGFuZGFyZCBjaXJjdWxhciBob2xsb3cgc3RlZWwgc2VjdGlvbnMgdG8gbWFpbnRhaW4gdGhlaXIgc2FtZSBpbmVydGlhIHdpdGggYSByZWR1Y3Rpb24gb2YgbWF0ZXJpYWwgdXNlLiAgIiwNCiAgICAiY29weXJpZ2h0IjogIkNvcHlyaWdodCBcdTAwQTkgMjAyNCBNYXJjbyBQYWxtYSIsDQogICAgImxpY2Vuc2UiOiAiTUlUIg0KICB9LA0KICAic2V0dGluZ3MiOiB7DQogICAgImJ1aWxkUGF0aCI6ICJmaWxlOi8vL0M6L1VzZXJzL21hcmNvL0RvY3VtZW50cy9fX0xhdm9yby9fX19WSUVOTkFfX18vMDFfUkVTRUFSQ0gvMDZfUFJPSkVDVFMvMjAyM19Vbmliby9wbHVnaW5fcmg4L2J1aWxkL3JoOF8xMiIsDQogICAgImJ1aWxkVGFyZ2V0Ijogew0KICAgICAgImhvc3QiOiB7DQogICAgICAgICJuYW1lIjogIlJoaW5vM0QiLA0KICAgICAgICAidmVyc2lvbiI6ICI4LjEyIg0KICAgICAgfSwNCiAgICAgICJ0aXRsZSI6ICJSaGlubzNEICg4LjEyKSIsDQogICAgICAic2x1ZyI6ICJyaDhfMTIiDQogICAgfSwNCiAgICAicHVibGlzaFRhcmdldCI6IHsNCiAgICAgICJ0aXRsZSI6ICJNY05lZWwgWWFrIFNlcnZlciINCiAgICB9DQogIH0sDQogICJjb2RlcyI6IFtdDQp9";
    static readonly dynamic s_projectServer = default;
    static readonly object s_project = default;

    static ProjectComponentPlugin()
    {
      s_projectServer = ProjectInterop.GetProjectServer();
      if (s_projectServer is null)
      {
        RhinoApp.WriteLine($"Error loading Grasshopper plugin. Missing Rhino3D platform");
        return;
      }

      // get project
      dynamic dctx = ProjectInterop.CreateInvokeContext();
      dctx.Inputs["projectAssembly"] = typeof(ProjectComponentPlugin).Assembly;
      dctx.Inputs["projectId"] = s_projectId;
      dctx.Inputs["projectData"] = s_projectData;

      object project = default;
      if (s_projectServer.TryInvoke("plugins/v1/deserialize", dctx)
            && dctx.Outputs.TryGet("project", out project))
      {
        // server reports errors
        s_project = project;
      }
    }

    public override GH_LoadingInstruction PriorityLoad()
    {
      if (AssemblyInfo.PluginCategoryIcon is SD.Bitmap icon)
      {
        Grasshopper.Instances.ComponentServer.AddCategoryIcon("Lattic3", icon);
      }
      Grasshopper.Instances.ComponentServer.AddCategorySymbolName("Lattic3", "Lattic3"[0]);

      return GH_LoadingInstruction.Proceed;
    }

    public static bool TryCreateScript(GH_Component ghcomponent, string serialized, out object script)
    {
      script = default;

      if (s_projectServer is null) return false;

      dynamic dctx = ProjectInterop.CreateInvokeContext();
      dctx.Inputs["component"] = ghcomponent;
      dctx.Inputs["project"] = s_project;
      dctx.Inputs["scriptData"] = serialized;

      if (s_projectServer.TryInvoke("plugins/v1/gh/deserialize", dctx))
      {
        return dctx.Outputs.TryGet("script", out script);
      }

      return false;
    }

    public static void DisposeScript(GH_Component ghcomponent, object script)
    {
      if (script is null)
        return;

      dynamic dctx = ProjectInterop.CreateInvokeContext();
      dctx.Inputs["component"] = ghcomponent;
      dctx.Inputs["project"] = s_project;
      dctx.Inputs["script"] = script;

      if (!s_projectServer.TryInvoke("plugins/v1/gh/dispose", dctx))
        throw new Exception("Error disposing Grasshopper script component");
    }
  }
}
