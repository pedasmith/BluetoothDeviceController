
## Supporting common graphing libraries

### **FAILURE** [Livecharts.dev](https://livecharts.dev/docs/winui/2.0.0/gallery)

Supports WinUI but seemingly WinUI2 and not WinUI3? They seem to require AvaloniaUI which is different? But [this blog post](https://xamlbrewer.wordpress.com/2023/12/04/displaying-charts-in-winui3-with-livecharts2/) says it works just fine?

1. [Installation](https://livecharts.dev/docs/Avalonia/2.0.0/Overview.Installation). I right-clicked on the project and selected "Manage NuGet Packages ...". Then I picked *Browse* and searched for "LiveCharts". That got a bunch of projects, none of them for WinUI3. So I searched for "LiveChartsCore.SkiaSharpView.Avalonia" and grabbed the package. This installed a metric ton of weird-ass packages. And looking more closely, this is an "Avalonia" package, which is different from a "WinUI3" package.
2. Added *xmlns:lvc="using:LiveChartsCore.SkiaSharpView.Avalonia"* to the UserControl
3. Added a grid row 2 (height=*) to the primary data grid containing just an <lvc:CartesianControl>. This completely failed with the error "A value of type 'CartesianChart' cannot be added to a collection or dictionary of type UIElementCollection"
4. Removed  LiveCharts from my NuGet. This seemed to work cleanly?

### [Scottplot.Net](https://scottplot.net/quickstart/winui/)

Has direct support for WinUI3 (probably). Certainly the WinUI Quickstart says I have to use the Microsoft.WindowsAppSDK, which IIRC is WinUI3.

1. [Installation](https://scottplot.net/quickstart/winui/). Like with LiveCharts, I added ScottPlot.WinUI from the NuGet packafge manager
2. Added *xmlns:ScottPlot="using:ScottPlot.WinUI"*
3. Added * <ScottPlot:WinUIPlot Grid.Row="2"  x:Name="uiScottPlot"/>*. This starts off in an error (blue squiggles) about how WinUIPlot isn't a thing. But a rebuild included a ton of random packages and then it also failed. A ScottPlot can't be added to a list of UI Elements.
