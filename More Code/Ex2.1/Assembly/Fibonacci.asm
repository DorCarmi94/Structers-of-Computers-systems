@2
D=M
@n
M=D
@3
D=A
@n
M=M-D
@1
D=M
@array
M=D
D=0
A=M
M=1
A=A+1
M=1
A=A+1
D=A
@point
M=D
@n
D=M
@i
M=D
(LOOP)
@i
D=M
@ENDLOOP
D;JLT
@point
D=M
A=D-1
A=A-1
D=M
A=A+1
D=D+M
A=A+1
M=D
A=A+1
D=A
@point
M=D
@i
M=M-1
@LOOP
0;JMP
(ENDLOOP)
@END
(END)
0;JMP