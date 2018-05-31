@echo off

start cmd /C "python -m http.server"
"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" "http://localhost:8000"
