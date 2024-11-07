; Analizador Sintactico
; Analizador Semantico

extern printf
extern scanf
extern fflush
extern stdout

segment .data
	texto db "%d",0
	space db "", 10, 0
	textoFinal db "----LISTA DE VARIABLES----", 10, 0
	texto1 db "Si funciona", 10, 0
	i dd 0
	nombreVariable1 db "i = ", 0

segment .text
	global main

main:
	mov eax, 0
	push eax
	mov dword [i], eax
; do 1
_doIni1:
;WriteLine
	push texto1
	call printf
; Asignacion a i
	inc dword [i]
; Termina asignacion a i
	mov eax, [i]
	push eax
	mov eax, 4
	push eax
	pop eax
	pop ebx
	cmp ebx, eax
	jnc _doFin1
jmp _doIni1
_doFin1:
;Imprimiendo valores
	push space
	call printf
	push textoFinal
	call printf
	push nombreVariable1
	call printf
	mov eax, [i]
	push eax
	push texto
	call printf
	push space
	call printf
