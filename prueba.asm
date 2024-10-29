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
; Asignacion a y
	mov eax, 0
	push eax
	pop eax
	mov y, eax
; Termina asignacion a y
; while 0
_whileIni0:
	mov eax, y
	push eax
	mov eax, 5
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jne _whileFin0
; Asignacion a y
	inc y
; Termina asignacion a y
jmp _whileIni0
_whileFin0:

	INVOKE ExitProcess,0
main endp

end main
