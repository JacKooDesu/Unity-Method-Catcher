# Unity-Method-Catcher
Using HarmonyLib dynamic catch method call

## Unity version
2021.3.1f1

## Known issue

- Not compatible with project which has imported mono.cecil
- Harmony auto reference missing when using vscode with omnisharp(fixed: modified target framework version to 4.7.2 manually)
- Editor GUI still buggy if array element is greater than 1
