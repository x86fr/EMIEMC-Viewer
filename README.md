# EMIEMC Viewer

A Windows desktop viewer for EMI scan results. It plots a measured spectrum against its CISPR / EN 55032 limit, lists the peaks with their margins, and shows a PASS / FAIL verdict. The chart can be exported for test reports.

It reads two file formats, independently:

| Source | Extension | Content |
|---|---|---|
| Tektronix SignalVu-PC (EMC-EMI) | `.emcemi` | XML trace (RSA306B and similar) |
| [TeraScan](#terascan-tb-files) | `.tb` | JSON session: traces, limit lines, peak list, verdict data |

The format is detected from the file content, not the extension.

## Usage

1. Pick **Graph #1** (main result) or **Graph #2** (overlay) in the *Files* box.
2. Click **Open File** or drag and drop a file onto the window.

Graph #1 draws the trace, its limit line, the margin line (dashed, 6 dB below the limit by default), the peak markers, the peak list and the verdict. Loading a new Graph #1 clears the overlay.

Graph #2 adds a second trace (for example before/after a fix, or H vs V polarization) on the same chart. Both files must start at the same frequency. Formats can be mixed, e.g. a TeraScan main graph with a Tektronix overlay.

### Controls

| Group | Control | Effect |
|---|---|---|
| Chart | X Axis Log | Logarithmic or linear frequency axis |
| | Y at 0 | Start the level axis at 0 instead of just below the trace minimum |
| | Theme | Dark (default) or Light, for screens or printed reports |
| Legends | Set Legend #1 / #2 | Rename a trace in the legend (defaults to the file name) |
| | Set Title | Add a title on the chart (it can be dragged) |
| | Disable Legend | Hide the legend |
| Limits | Detector | Quasi-Peak / Average: picks the conducted limit, or the trace in a two-trace TeraScan file |
| | Force Fail | Force the verdict to FAIL |
| | Save to File | Save the chart as `<file name> <timestamp>.png` next to the Graph #1 file |
| | Save to Clipboard | Copy the chart image to the clipboard |

The peak list, the verdict and the title can be moved with the mouse before exporting. The log pane shows what was read from the file: units, instrument, transducers, the limit chosen, and the peaks.

## How results are evaluated

### Tektronix `.emcemi` files

- **Units:** `dBuV` and `dBuV/m` are used as-is. `dBm` is converted by adding 107 dB (dBm → dBµV into 50 Ω), with no antenna factor applied. Other units are rejected.
- **Limit:** taken from the built-in table below, matched on the trace's start frequency. For conducted scans (150 kHz start), the limit is QP if the file name contains `QP` and AVG otherwise. The Detector box can switch it afterwards.
- **Peaks:** the trace is cut into ranges whose width is the decade's power of ten (10 MHz wide from 30 to 100 MHz, then 100 MHz wide up to 1 GHz). The highest point of each range is a peak. Peaks are sorted by margin and the six worst are shown.
- **Verdict:** **FAIL** if any shown peak is above the limit. **FAIL?** if any is within 6 dB of it. **PASS** otherwise.

### TeraScan `.tb` files

- **Limit and margin line:** the enabled limit line(s) and the margin offset stored in the session are used. If the file has none, the built-in table is used as for Tektronix files.
- **Peaks:** the first six peaks of TeraScan's peak list are shown, in TeraScan's order (worst margin first), including the QP final level, polarization, turntable angle and mast height when they were measured. The margin uses the QP level when one was measured, otherwise the Max level. A file without peaks falls back to the viewer's own peak search.
- **Verdict:** TeraScan's own judgement is used: **FAIL** if any peak is flagged as over its limit, **PASS** otherwise. A peak whose QP final passed is not counted as a failure even if its Max reading was over. The verdict covers the whole file. If no peak was checked against a limit, the viewer computes the verdict itself with the Tektronix rule, using the file's margin offset instead of 6 dB.
- **Two-trace sessions** (the conducted QP + AVG suite): one trace is shown at a time, Quasi-Peak first. The **Detector** box switches to the other trace, along with its limit line and peaks.

TeraScan session format versions 1 and 2 are supported. Newer versions are still opened, with a warning in the log. Trace units `DbuV`, `DbuVm` and `Dbm` are supported.

## Built-in limits

Used for Tektronix files and for TeraScan files that carry no limit line.

| Limit | Start frequency | Range |
|---|---|---|
| CISPR 32 Class B conducted, QP | 150 kHz | 150 kHz – 30 MHz |
| CISPR 32 Class B conducted, AVG | 150 kHz | 150 kHz – 30 MHz |
| CISPR 32 Class B radiated, 3 m | 30 MHz | 30 MHz – 1 GHz |
| CISPR 32 Class B radiated, 3 m | 1 GHz | 1 GHz – 6 GHz |

A scan that does not start at one of these frequencies has no built-in limit, and the viewer refuses to draw it (see the log). Limits are defined in [EMILimits.cs](EMILimits.cs).

## Requirements

- Windows with .NET Framework 4.8. This is included in Windows 10 version 1903 and later, and in Windows 11.

## Building

Open `EMIEMC Viewer.sln` in Visual Studio 2019 or later and build. Alternatively, from a Developer Command Prompt:

```
msbuild "EMIEMC Viewer.sln" -p:Configuration=Release
```

The output is `bin\Release\EMIEMC Viewer.exe` and its `.exe.config`. There are no extra DLLs to deploy.

## Project layout

| File | Role |
|---|---|
| `Form1.cs` | UI: chart drawing, limits, peaks, verdict, export |
| `MeasurementLoader.cs` | Detects the file format and calls the right loader |
| `TektronixLoader.cs`, `RSADecoder.cs` | `.emcemi` reader (`RSAPersist` XML) |
| `TeraScanLoader.cs` | `.tb` reader (JSON) |
| `EmiMeasurement.cs` | Format-neutral model shared by both loaders (traces, limits, peaks, verdict) and limit interpolation |
| `EMILimits.cs` | Built-in CISPR 32 limit table |
| `Peaks.cs` | Peak record used by the viewer's own peak search |

To add a new input format, write a loader that returns an `EmiMeasurement` and hook it into `MeasurementLoader`.

## Known limitations

- An overlay must start at exactly the same frequency as Graph #1.
- Changing the Detector selection redraws Graph #1 and clears the overlay.
- The `dBm` → `dBµV` conversion assumes a 50 Ω input and applies no transducer or antenna factor.
