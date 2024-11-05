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
	mov eax, 3
	push eax
	mov eax, 5
	push eax
	pop ebx
	pop eax
	add eax, ebx
	push eax
	mov eax, 8
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	mov eax, 10
	push eax
	mov eax, 4
	push eax
	pop ebx
	pop eax
	sub eax, ebx
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	div ebx
	push eax
	pop ebx
	pop eax
	sub eax, ebx
	push eax
	pop eax
	mov x, eax
; Termina asignacion a x
; Asignacion a x
	inc x
; Termina asignacion a x
; if 1
	mov eax, x
	push eax
	mov eax, 62
	push eax
	pop eax
	pop ebx
	cmp ebx, eax
	jnZ _elseIf1
; Asignacion a x
	mov eax, 0
	push eax
	pop eax
	mov x, eax
; Termina asignacion a x
; if 2
	mov eax, x
	push eax
	mov eax, 0
	push eax
	pop eax
	pop ebx
	cmp ebx, eax
	je _elseIf2
; Asignacion a x
	mov eax, 1
	push eax
	pop eax
	mov x, eax
; Termina asignacion a x
jmp _finIf2
_elseIf2:
_finIf2:
jmp _finIf1
_elseIf1:
_finIf1:
; Asignacion a x
	mov eax, 0
	push eax
	pop eax
	mov x, eax
; Termina asignacion a x
; while 1
_whileIni1:
	mov eax, x
	push eax
	mov eax, 10
	push eax
	pop eax
	pop ebx
	cmp ebx, eax
	je _whileFin1
; Asignacion a x
	inc x
; Termina asignacion a x
jmp _whileIni1
_whileFin1:

	INVOKE ExitProcess,0
main endp

end main
