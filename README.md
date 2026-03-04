<p align="center">
  <img src="https://avatars.githubusercontent.com/u/209633456?v=4" width="160" alt="RedTail Indicators Logo"/>
</p>

<h1 align="center">RedTail Key Levels</h1>

<p align="center">
  <b>An all-in-one reference level indicator for NinjaTrader 8.</b><br>
  Pivot points, prior session ranges, Monday range, Globex range, RTH range, Fibonacci retracements, and level merging — one indicator, every level you need.
</p>

<p align="center">
  <a href="https://buymeacoffee.com/dmwyzlxstj">
    <img src="https://img.shields.io/badge/☕_Buy_Me_a_Coffee-FFDD00?style=flat-square&logo=buy-me-a-coffee&logoColor=black" alt="Buy Me a Coffee"/>
  </a>
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/3astbeast/RedTail-Key-Levels/refs/heads/main/Screenshot%202026-03-03%20131356.png" width="800" alt="RedTail Key Levels Screenshot"/>
</p>

---

## Overview

RedTail Key Levels consolidates every commonly referenced support/resistance level into a single indicator. Instead of stacking multiple indicators on your chart for pivots, prior day/week/month ranges, Monday range, Globex range, RTH session range, and Fibonacci retracements, this draws them all with a unified level merging system that combines overlapping levels into clean, labeled lines. Every level category can be independently toggled on or off.

---

## Pivot Points

Standard pivot point calculations with three resistance levels (R1, R2, R3), three support levels (S1, S2, S3), and the central Pivot Point (PP).

**Pivot Range:** Daily, Weekly, or Monthly — determines which period's HLC data is used for the pivot calculation.

**HLC Mode:**
- **Calc from Intraday Data** — Computes the prior period's high, low, and close from your chart's intraday bars
- **Daily Bars** — Uses a secondary daily data series for the HLC values
- **User Defined Values** — Manually enter custom high, low, and close values

**Midlines** — Optional midline levels between each pivot level (R1M, R2M, R3M, S1M, S2M, S3M) for finer reference points.

**Show Historical** — Optionally display pivot levels from prior periods in addition to the current period.

---

## Prior Session Ranges

**Previous Day High/Low (PDH/PDL)** — The high and low from the prior trading session.

**Previous Week High/Low (PWH/PWL)** — The high and low from the prior trading week (Sunday 6 PM – Friday 5 PM ET).

**Previous Month High/Low (PMH/PML)** — The high and low from the prior calendar month.

Each pair can be independently toggled on or off.

---

## Monday Range

The high and low from Monday's session (Sunday 6 PM – Monday 5 PM ET). Monday's range often defines the initial weekly range and serves as a key reference for the rest of the week.

---

## Globex Session Range

The high and low of the current Globex (electronic) session spanning Sunday 6 PM – Friday 5 PM ET. Tracks the full weekly electronic session range.

---

## RTH Session Range

The high and low of the current Regular Trading Hours session (9:30 AM – 4:00 PM ET, Monday through Friday). All session times are EST/ET timezone-aware.

---

## Fibonacci Retracements

Up to 10 customizable Fibonacci levels calculated from the prior day's range. Set any level to 0.0 to disable it.

**Default levels:** 23.6%, 38.2%, 50%, 61.8%, 78.6% (5 enabled, 5 available slots disabled by default).

Fib levels are drawn from the prior day's high to low and update automatically with each new session.

---

## Level Merging

When multiple levels from different categories land at or near the same price, they can be merged into a single line with a combined label (e.g., "PDH/R2/PWH"). This reduces visual clutter while preserving the information about which levels are confluent — one of the most useful features for identifying high-probability zones.

**Merge Tolerance** — Configurable in ticks. Levels within this distance are combined into a single merged line. Set to 0 to disable merging.

---

## Line Width

Configurable number of bars to extend levels backward from the current bar. Set to 0 for infinite extension to the chart edge.

---

## Plot Outputs

All 33 levels are exposed as named plot series, usable by strategies or other indicators:

- PP, R1, R2, R3, S1, S2, S3 (pivot levels)
- R1M, R2M, R3M, S1M, S2M, S3M (midlines)
- PDH, PDL, PWH, PWL, PMH, PML (prior session ranges)
- MH, ML (Monday range)
- GH, GL (Globex range)
- NYH, NYL (RTH session range)
- Fib1–Fib10 (Fibonacci levels)

---

## Installation

1. Download the .cs file from the indicator's repository
2. Copy the .cs to documents\Ninja Trader 8\bin\custom\indicators
3. Open Ninja Trader (if not already open) 
4. In control center, go to New --> Ninja Script Editor
5. Expand the Indicator Tree, find your new indicator, double click to open it
6. At the top of the Editor window, click the "Compile" button
7. That's it!

---

## Part of the RedTail Indicators Suite

This indicator is part of the [RedTail Indicators](https://github.com/3astbeast/RedTailIndicators) collection — free NinjaTrader 8 tools built for futures traders who demand precision.

---

<p align="center">
  <a href="https://buymeacoffee.com/dmwyzlxstj">
    <img src="https://img.shields.io/badge/☕_Buy_Me_a_Coffee-Support_My_Work-FFDD00?style=for-the-badge&logo=buy-me-a-coffee&logoColor=black" alt="Buy Me a Coffee"/>
  </a>
</p>
