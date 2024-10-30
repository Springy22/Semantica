; Analizador Sintactico
; Analizador Semantico

.model flat,stdcall
.stack 4096

.data
	x dd 0
	y dd 0

.code
ExitProcess PROTO, dwExitCode:DWORD
main proc
; for 1
; Asignacion a y
	mov eax, 3
	push eax
	pop eax
	mov y, eax
; Termina asignacion a y
_forIni1:
	mov eax, y
	push eax
	mov eax, 1
	push eax
	pop eax
	pop ebx
	cmp ebx, eax
	jz _forFin1
	jc _forFin1
; Asignacion a y
	dec y
; Termina asignacion a y
jmp _forIni1
_forFin1:

	INVOKE ExitProcess,0
main endp

end main
