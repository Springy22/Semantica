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
; Asignacion a x
	;Usando Read
	pop eax
	mov eax, bufferChar0
	push eax
	push formatChar0
	call scanf
	add esp, 8
; Termina asignacion a x

	INVOKE ExitProcess,0
main endp

end main
