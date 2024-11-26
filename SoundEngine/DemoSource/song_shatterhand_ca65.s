; This file is for the FamiStudio Sound Engine and was generated by FamiStudio

.if FAMISTUDIO_CFG_C_BINDINGS
.export _music_data_shatterhand=music_data_shatterhand
.endif

music_data_shatterhand:
	.byte 1
	.word @instruments
	.word @samples-4
; 00 : Final Area
	.word @song0ch0
	.word @song0ch1
	.word @song0ch2
	.word @song0ch3
	.word @song0ch4
	.byte .lobyte(@tempo_env_1_mid), .hibyte(@tempo_env_1_mid), 0, 0

.export music_data_shatterhand
.global FAMISTUDIO_DPCM_PTR

@instruments:
	.word @env14,@env7,@env9,@env0 ; 00 : CymbalLow
	.word @env5,@env1,@env9,@env0 ; 01 : BassDrum
	.word @env13,@env8,@env9,@env0 ; 02 : Snare
	.word @env10,@env8,@env9,@env0 ; 03 : CymbalHigh
	.word @env2,@env6,@env12,@env4 ; 04 : Lead-Duty1
	.word @env2,@env6,@env9,@env4 ; 05 : Lead-Duty0
	.word @env2,@env6,@env11,@env4 ; 06 : Lead-Duty2
	.word @env3,@env6,@env9,@env0 ; 07 : Bass

@env0:
	.byte $00,$c0,$7f,$00,$02
@env1:
	.byte $c0,$bf,$c1,$00,$02
@env2:
	.byte $06,$c8,$c9,$c5,$00,$03,$c4,$c4,$c2,$00,$08
@env3:
	.byte $00,$cf,$7f,$00,$02
@env4:
	.byte $00,$c0,$08,$c0,$04,$bd,$03,$bd,$00,$03
@env5:
	.byte $00,$cf,$ca,$c3,$c2,$c0,$00,$05
@env6:
	.byte $c0,$7f,$00,$01
@env7:
	.byte $c0,$c2,$c5,$00,$02
@env8:
	.byte $c0,$c1,$c2,$00,$02
@env9:
	.byte $7f,$00,$00
@env10:
	.byte $00,$cb,$ca,$09,$c9,$00,$04
@env11:
	.byte $c2,$7f,$00,$00
@env12:
	.byte $c1,$7f,$00,$00
@env13:
	.byte $00,$ca,$c6,$c3,$c0,$00,$04
@env14:
	.byte $00,$cb,$cb,$c5,$03,$c4,$03,$c3,$03,$c2,$00,$09

@samples:

@tempo_env_1_mid:
	.byte $03,$05,$80

@song0ch0:
@song0ch0loop:
	.byte $47, .lobyte(@tempo_env_1_mid), .hibyte(@tempo_env_1_mid), $7e, $8a
@song0ref7:
	.byte $16, $9b, $45, $83, $19, $89, $45, $81, $43, $16, $16, $9b, $45, $83, $1b, $89, $45, $81, $43, $16, $16, $9b, $45, $83
	.byte $1e, $89, $45, $81, $43, $16, $16, $9b, $45, $83, $1d, $9b, $45, $83, $1b, $89, $45, $81, $43, $1d
@song0ref51:
	.byte $19, $89, $45, $81, $43, $1b, $14, $89, $45, $81, $43, $19, $48
	.byte $41, $32
	.word @song0ref7
	.byte $48, $00, $43, $19, $8d, $27, $89, $45, $81, $43, $14
@song0ref78:
	.byte $25, $89, $45, $81, $43, $27, $20, $89, $45, $81, $43, $25, $27, $89, $45, $81, $43, $20
	.byte $41, $0a
	.word @song0ref78
	.byte $1b, $89, $45, $81, $43, $20
	.byte $41, $0a
	.word @song0ref51
	.byte $1d, $89, $45, $81, $43, $14, $19, $89, $45, $81, $43, $1d, $14, $89, $45, $81, $43, $19, $11, $89, $45, $81, $43, $14
@song0ref132:
	.byte $14, $89, $45, $81, $43, $11, $16, $89, $45, $81, $43, $14, $48, $27, $89, $45, $81, $43, $16, $00, $43, $16, $8d, $1e
	.byte $89, $45, $81, $43, $27, $00, $43, $27
@song0ref164:
	.byte $8d, $43, $1e, $8f, $20, $89, $45, $81, $43, $1e, $00, $43, $1e, $8d, $43, $20, $8f, $1d, $89, $45, $81, $43, $20, $00
	.byte $43, $20, $8d, $43, $1d, $8f, $1e, $89, $45, $81, $43, $1d, $00, $43, $1d, $8d, $1b, $ad, $45, $83, $48, $1d, $89, $45
	.byte $81, $43, $1b, $00, $43, $1b, $8d, $19, $89, $45, $81, $43, $1d, $1b, $d1, $45, $83, $00, $43, $1b, $8d, $11, $89, $45
	.byte $81, $43, $1b
	.byte $41, $0a
	.word @song0ref132
	.byte $1b, $89, $45, $81, $43, $16
	.byte $41, $0a
	.word @song0ref51
@song0ref251:
	.byte $16, $89, $45, $81, $43, $14, $48, $00, $43, $14, $8d, $43, $16, $8f, $1e, $89, $45, $81, $43, $16, $00, $43, $16
	.byte $41, $3c
	.word @song0ref164
	.byte $41, $0a
	.word @song0ref132
	.byte $1b, $89, $45, $81, $43, $16
	.byte $41, $0a
	.word @song0ref51
	.byte $41, $0a
	.word @song0ref251
	.byte $88
@song0ref293:
	.byte $23, $ad, $45, $83, $25, $89, $45, $81, $43, $23
@song0ref303:
	.byte $27, $89, $45, $81, $43, $25, $29, $89, $45, $81, $43, $27, $2a, $9b, $45, $83
@song0ref319:
	.byte $2c, $89, $45, $81, $43, $2a, $2a, $89, $45, $81, $43, $2c, $00, $43, $2c, $8d, $26, $ad, $45, $83, $48, $23, $bf, $45
	.byte $83, $29, $8f, $45, $81, $43, $23, $27, $8f, $45, $81, $43, $29, $26, $8f, $45, $81, $43, $27, $27, $9b, $45, $83, $22
	.byte $89, $45, $81, $43, $27, $2a, $9b, $45, $83
@song0ref376:
	.byte $29, $89
@song0ref378:
	.byte $45, $81, $43, $2a, $27, $89, $45, $81, $43, $29, $25, $89, $45, $81, $43, $27, $48, $00, $43, $27, $8d, $43, $25, $8f
	.byte $41, $13
	.word @song0ref293
	.byte $25, $9b, $45, $83
	.byte $41, $0a
	.word @song0ref303
	.byte $00, $43, $27, $8d, $2a, $ad, $45, $83, $48, $2c, $bf
@song0ref423:
	.byte $45, $83, $2c, $89, $45, $83, $2e, $89, $45, $81, $43, $2c, $31, $89, $45, $81, $43, $2e, $2c, $89, $45, $81, $43, $31
	.byte $00, $43, $31, $8d, $8a, $1e, $89, $45, $81, $43, $2c
@song0ref458:
	.byte $1d, $89, $45, $81, $43, $1e, $19, $89, $45, $81, $43, $1d, $12, $89, $45, $81, $43, $19, $11, $89, $45, $81, $43, $12
	.byte $41, $55
	.word @song0ref132
	.byte $41, $0a
	.word @song0ref132
	.byte $1b, $89, $45, $81, $43, $16
	.byte $41, $0a
	.word @song0ref51
	.byte $41, $11
	.word @song0ref251
	.byte $41, $3c
	.word @song0ref164
	.byte $41, $0a
	.word @song0ref132
	.byte $1b, $89, $45, $81, $43, $16
	.byte $41, $0a
	.word @song0ref51
	.byte $41, $0a
	.word @song0ref251
	.byte $25, $9b, $45, $83, $25, $89, $45, $83
	.byte $41, $0a
	.word @song0ref303
	.byte $29, $ad, $45, $83, $29, $89, $45, $83, $27, $89, $45, $81, $43, $29, $00, $43, $29, $8d, $26, $ad, $45, $83, $48, $27
	.byte $bf, $45, $83, $27, $89
@song0ref558:
	.byte $45, $83, $29, $89, $45, $81, $43, $27, $2a, $89, $45, $81, $43, $29, $2c, $ad, $45, $83, $2a, $89, $45, $81, $43, $2c
	.byte $29, $89, $45, $81, $43, $2a, $00, $43, $2a, $8d, $27, $ad, $45, $83, $48, $00, $43, $27, $9f
@song0ref601:
	.byte $29, $9b, $45, $83, $29, $89, $45, $83, $2a, $89, $45, $81, $43, $29, $2c, $89, $45, $81, $43, $2a, $2e, $ad, $45, $83
	.byte $2c, $89, $45, $81, $43, $2e, $2a, $89, $45, $81, $43, $2c, $00, $43, $2c, $8d
	.byte $41, $0a
	.word @song0ref319
	.byte $29, $89, $45, $81, $43, $2a, $48, $26, $bf, $45, $83, $27, $bf, $45, $83, $29, $89, $45, $81, $43, $27, $29, $8b, $45
	.byte $81, $00, $43, $29, $8d, $29, $d1, $45, $83, $48, $2e, $89, $45, $81, $43, $29
@song0ref684:
	.byte $27, $89, $45, $81, $43, $2e, $22, $89, $45, $81, $43, $27, $2c, $89, $45, $81, $43, $22, $25, $89, $45, $81, $43, $2c
	.byte $20, $89, $45, $81, $43, $25, $1e, $9b, $45, $83, $00, $43, $1e, $8d, $1e, $89, $45, $83
	.byte $41, $0a
	.word @song0ref458
	.byte $1e, $89, $45, $81, $43, $19
	.byte $41, $0a
	.word @song0ref458
@song0ref738:
	.byte $1b, $89, $45, $81, $43, $19, $48, $1e, $89, $45, $81, $43, $1b, $1e, $89, $45, $83, $00, $43, $1e, $8d, $20, $89, $45
	.byte $81, $43, $1e, $20, $89, $45, $83, $00, $43, $20, $8d, $1e, $89, $45, $81, $43, $20, $1e, $89, $45, $83, $00, $43, $1e
	.byte $8d, $20, $89, $20, $81, $43, $1e, $20, $89, $45, $83, $00, $43, $20, $8d, $27, $89, $45, $81, $43, $20, $25, $89, $45
	.byte $81, $43, $27, $23, $89, $45, $81, $43, $25, $22, $89, $45, $81, $43, $23, $48, $2e, $89, $45, $81, $43, $22
	.byte $41, $24
	.word @song0ref684
	.byte $41, $0a
	.word @song0ref458
	.byte $1e, $89, $45, $81, $43, $19
	.byte $41, $0a
	.word @song0ref458
	.byte $41, $49
	.word @song0ref738
	.byte $48, $17, $9b, $45, $83, $1e, $89, $45, $81, $43, $17, $19, $9b, $45, $83, $20, $9b, $45, $83, $1b, $9b, $45, $83, $22
	.byte $9b, $45, $83, $29, $89, $45, $81, $43, $22, $2a, $89, $45, $81, $29, $8b
	.byte $41, $0d
	.word @song0ref378
	.byte $48, $2a, $89, $45, $81, $43, $25, $2a, $89, $45, $83, $00, $43, $2a, $8d, $29, $89, $45, $81, $43, $2a
@song0ref913:
	.byte $29, $89, $45, $83, $00, $43, $29, $8d, $2a, $89, $45, $81, $43, $29, $2a, $89, $45, $83, $00, $43, $2a, $8d, $11, $89
	.byte $45, $81, $43, $2a, $16, $89, $45, $81, $43, $11, $1d, $89, $45, $81, $43, $16, $25, $89, $45, $81, $43, $1d, $29, $ad
	.byte $45, $83, $42
	.word @song0ch0loop
@song0ch1:
@song0ch1loop:
	.byte $88
@song0ref968:
	.byte $1b, $9b, $45, $83, $1e, $89, $45, $81, $43, $1b, $1b, $9b, $45, $83, $20, $89, $45, $81, $43, $1b, $1b, $9b, $45, $83
	.byte $22, $89, $45, $81, $43, $1b, $1b, $9b, $45, $83, $20, $9b, $45, $83, $1e, $89, $45, $81, $43, $20
	.byte $41, $0a
	.word @song0ref458
	.byte $41, $28
	.word @song0ref968
	.byte $41, $0a
	.word @song0ref458
	.byte $00, $43, $1d, $8d, $2a, $89, $45, $81, $43, $19
@song0ref1031:
	.byte $29, $89, $45, $81, $43, $2a, $25, $89, $45, $81, $43, $29, $2a, $89, $45, $81, $43, $25
	.byte $41, $0a
	.word @song0ref1031
	.byte $1e, $89, $45, $81, $43, $25
	.byte $41, $0a
	.word @song0ref458
	.byte $20, $89, $45, $81, $43, $19, $1d, $89, $45, $81, $43, $20, $19, $89, $45, $81, $43, $1d, $16, $89, $45, $81, $43, $19
@song0ref1085:
	.byte $19, $89, $45, $81, $43, $16
@song0ref1091:
	.byte $1b, $89, $45, $81, $43, $19, $33, $89, $45, $81, $43, $1b, $00, $43, $1b, $8d, $22, $89, $45, $81, $43, $33, $00, $43
	.byte $33
@song0ref1116:
	.byte $8d, $43, $22, $8f, $23, $89, $45, $81, $43, $22, $00, $43, $22, $8d, $43, $23, $8f, $20, $89, $45, $81, $43, $23, $00
	.byte $43, $23, $8d, $43, $20, $8f, $22, $89, $45, $81, $43, $20, $00, $43, $20, $8d, $1e, $ad, $45, $83, $20, $89, $45, $81
	.byte $43, $1e, $00, $43, $1e, $8d, $1d, $89, $45, $81, $43, $20, $1e, $d1, $45, $83, $00, $43, $1e, $8d, $16, $89, $45, $81
	.byte $43, $1e
	.byte $41, $0a
	.word @song0ref1085
	.byte $1e, $89, $45, $81, $43, $1b
	.byte $41, $0a
	.word @song0ref458
@song0ref1202:
	.byte $1b, $89, $45, $81, $43, $19, $00, $43, $19, $8d, $43, $1b, $8f, $22, $89, $45, $81, $43, $1b, $00, $43, $1b
	.byte $41, $3c
	.word @song0ref1116
	.byte $41, $0a
	.word @song0ref1085
	.byte $1e, $89, $45, $81, $43, $1b
	.byte $41, $0a
	.word @song0ref458
	.byte $41, $0a
	.word @song0ref1202
	.byte $8c, $27, $ad
	.byte $41, $0d
	.word @song0ref558
@song0ref1248:
	.byte $89, $45, $81, $43, $2a, $2e, $9b, $45, $83
@song0ref1257:
	.byte $2f, $89, $45, $81, $43, $2e, $2e, $89, $45, $81, $43, $2f, $00, $43, $2f, $8d, $29, $ad, $45, $83, $2c, $bf, $45, $83
	.byte $2c, $8f, $45, $83, $2a, $8f, $45, $81, $43, $2c, $29, $8f, $45, $81, $43, $2a, $2a, $9b, $45, $83, $27, $89, $45, $81
	.byte $43, $2a, $2e, $d1, $45, $83, $00, $43, $2e, $9f, $27, $ad
	.byte $41, $0d
	.word @song0ref558
	.byte $41, $0d
	.word @song0ref1248
	.byte $31, $89, $45, $81, $43, $2f, $00, $43, $2f, $8d, $33, $ad, $45, $83, $35, $bf, $45, $83, $35, $89, $45, $83, $36, $89
	.byte $45, $81, $43, $35, $38, $89, $45, $81, $43, $36, $35, $89, $45, $81, $43, $38, $00, $43, $38, $8d, $88, $2a, $89, $45
	.byte $81, $43, $35
	.byte $41, $0a
	.word @song0ref1031
	.byte $1e, $89, $45, $81, $43, $25
	.byte $41, $0a
	.word @song0ref458
	.byte $41, $50
	.word @song0ref1091
	.byte $41, $0a
	.word @song0ref1085
	.byte $1e, $89, $45, $81, $43, $1b
	.byte $41, $0a
	.word @song0ref458
	.byte $41, $11
	.word @song0ref1202
	.byte $41, $3c
	.word @song0ref1116
	.byte $41, $0a
	.word @song0ref1085
	.byte $1e, $89, $45, $81, $43, $1b
	.byte $41, $0a
	.word @song0ref458
	.byte $41, $0a
	.word @song0ref1202
	.byte $41, $23
	.word @song0ref601
	.byte $29, $ad, $45, $83, $2a, $bf, $45, $83, $2a, $8b, $45, $81, $2c, $89, $45, $81, $43, $2a, $2e, $89, $45, $81, $43, $2c
	.byte $2f, $ad, $45, $83, $2e, $89, $45, $81, $43, $2f, $2c, $89, $45, $81, $43, $2e, $00, $43, $2e, $8d, $2a, $ad, $45, $83
	.byte $00, $43, $2a, $9f, $2c, $9b
	.byte $41, $0b
	.word @song0ref423
	.byte $2f, $89, $45, $81, $43, $2e, $31, $ad, $45, $83, $2f, $89, $45, $81, $43, $31, $2e, $89, $45, $81, $43, $2f, $00, $43
	.byte $2f, $8d
	.byte $41, $0a
	.word @song0ref1257
	.byte $2c, $89, $45, $81, $43, $2e, $2e, $bf, $45, $83, $30, $bf, $45, $83, $31, $89, $45, $81, $43, $30, $31, $89, $45, $83
	.byte $00, $43, $31, $8d, $32, $d1, $45, $83, $33, $89, $45, $81, $43, $32
@song0ref1549:
	.byte $2e, $89, $45, $81, $43, $33, $27, $89, $45, $81, $43, $2e, $31, $89, $45, $81, $43, $27, $2c, $89, $45, $81, $43, $31
	.byte $25, $89, $45, $81, $43, $2c, $23, $9b, $45, $83, $00, $43, $23, $8d, $2a, $89, $45, $81, $43, $23
	.byte $41, $0f
	.word @song0ref1031
	.byte $41, $0a
	.word @song0ref1031
@song0ref1599:
	.byte $27, $89, $45, $81, $43, $25, $23, $89, $45, $81, $43, $27, $23, $89, $45, $83, $00, $43, $23, $8d, $25, $89, $45, $81
	.byte $43, $23, $25, $89, $45, $83, $00, $43, $25, $8d, $27, $89, $45, $81, $43, $25, $27, $89, $45, $83, $00, $43, $27, $8d
	.byte $29, $89, $45, $81, $43, $27
	.byte $41, $0c
	.word @song0ref913
	.byte $41, $0f
	.word @song0ref376
	.byte $33, $89, $45, $81, $43, $25
	.byte $41, $25
	.word @song0ref1549
	.byte $41, $0f
	.word @song0ref1031
	.byte $41, $0a
	.word @song0ref1031
	.byte $41, $2e
	.word @song0ref1599
	.byte $41, $0c
	.word @song0ref913
	.byte $41, $0f
	.word @song0ref376
	.byte $23, $9b, $45, $83, $2a, $89, $45, $81, $43, $23, $25, $9b, $45, $83, $2c, $9b, $45, $83, $27, $9b, $45, $83, $2e, $9b
	.byte $45, $83, $35, $89, $45, $81, $43, $2e, $36, $89, $45, $81, $43, $35, $35, $89, $45, $81, $43, $36, $33, $89, $45, $81
	.byte $43, $35, $31, $89, $45, $81, $43, $33
@song0ref1739:
	.byte $33, $89, $45, $81, $43, $31, $33, $89, $45, $83, $00, $43, $33, $8d, $31, $89, $45, $81, $43, $33, $31, $89, $45, $83
	.byte $00, $43, $31, $8d
	.byte $41, $0c
	.word @song0ref1739
	.byte $16, $89, $45, $81, $43, $33, $1d, $89, $45, $81, $43, $16, $22, $89, $45, $81, $43, $1d, $29, $89, $45, $81, $43, $22
	.byte $2e, $ad, $45, $83, $42
	.word @song0ch1loop
@song0ch2:
@song0ch2loop:
	.byte $8e
@song0ref1803:
	.byte $1b
@song0ref1804:
	.byte $8b
@song0ref1805:
	.byte $00, $81, $1b, $8b, $00, $81, $1b, $8b, $00, $81
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $1b, $8b, $00, $81, $1b, $8b, $00, $81
@song0ref1829:
	.byte $19, $9d
@song0ref1831:
	.byte $00, $81, $16, $8b, $00, $81, $19, $8b, $00, $81, $1d
	.byte $41, $0b
	.word @song0ref1804
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0d
	.word @song0ref1829
	.byte $8b, $00, $81, $19, $f7, $00, $81, $19, $d3, $00, $81, $19, $8b
	.byte $41, $0a
	.word @song0ref1831
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $1b, $8b, $00, $81, $1b, $8b
	.byte $41, $0a
	.word @song0ref1831
@song0ref1906:
	.byte $1b, $8b, $00, $81
@song0ref1910:
	.byte $1e
@song0ref1911:
	.byte $8b, $00, $81, $1d
@song0ref1915:
	.byte $8b, $00, $81, $19, $8b, $00, $81, $1b
@song0ref1923:
	.byte $8b, $00, $81, $17, $8b, $00, $81, $17, $8b, $00, $81, $17
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $8b, $00, $81, $17, $8b
	.byte $41, $0a
	.word @song0ref1831
	.byte $41, $14
	.word @song0ref1906
@song0ref1967:
	.byte $14, $8b, $00, $81, $14, $8b, $00, $81, $1b, $8b, $00, $81, $1e, $8b, $00, $81
	.byte $41, $10
	.word @song0ref1967
@song0ref1986:
	.byte $16
@song0ref1987:
	.byte $8b, $00, $81, $16, $8b, $00, $81, $1d, $8b, $00, $81, $20, $8b, $00, $81
	.byte $41, $10
	.word @song0ref1986
	.byte $41, $10
	.word @song0ref1967
	.byte $41, $10
	.word @song0ref1986
@song0ref2011:
	.byte $1b, $8b, $00, $81, $1b, $8b, $00, $81, $19, $8b, $00, $81, $19
	.byte $41, $0b
	.word @song0ref1923
	.byte $16, $8b, $00, $81, $16, $8b, $00, $81
	.byte $41, $10
	.word @song0ref1967
	.byte $41, $10
	.word @song0ref1967
	.byte $41, $10
	.word @song0ref1986
	.byte $41, $10
	.word @song0ref1986
	.byte $17
@song0ref2048:
	.byte $8b, $00, $81, $17, $8b, $00, $81, $1e, $8b, $00, $81, $23
	.byte $41, $0c
	.word @song0ref2048
	.byte $8b, $00, $81, $19, $8b, $00, $93, $19, $8b, $00, $81, $19, $9d, $00, $81, $19, $8b, $00, $81, $19, $9d
	.byte $41, $0a
	.word @song0ref1805
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $16
	.byte $41, $0b
	.word @song0ref1915
	.byte $41, $19
	.word @song0ref1910
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0c
	.word @song0ref1923
	.byte $8b, $00, $81, $17, $8b
	.byte $41, $0a
	.word @song0ref1831
	.byte $41, $14
	.word @song0ref1906
@song0ref2150:
	.byte $19
@song0ref2151:
	.byte $8b, $00, $81, $19, $8b, $00, $81, $19, $8b, $00, $81
	.byte $41, $0c
	.word @song0ref2150
	.byte $19, $8b, $00, $81, $19
@song0ref2170:
	.byte $8b, $00, $81, $1a, $8b, $00, $81, $1a, $8b, $00, $81, $1a
	.byte $41, $0c
	.word @song0ref2170
	.byte $41, $0b
	.word @song0ref2170
	.byte $41, $0c
	.word @song0ref1803
	.byte $41, $0c
	.word @song0ref1803
	.byte $1b, $8b, $00, $81, $1b
@song0ref2199:
	.byte $8b, $00, $81, $1c, $8b, $00, $81, $1c, $8b, $00, $81, $1c
	.byte $41, $0c
	.word @song0ref2199
	.byte $41, $0b
	.word @song0ref2199
@song0ref2217:
	.byte $1d, $8b, $00, $81, $1d, $8b, $00, $81, $1d, $8b, $00, $81
	.byte $41, $0c
	.word @song0ref2217
	.byte $1d
	.byte $41, $0b
	.word @song0ref1911
	.byte $41, $0c
	.word @song0ref2150
	.byte $41, $0c
	.word @song0ref2150
	.byte $19
@song0ref2243:
	.byte $8b, $00, $81, $16, $8b, $00, $81, $16, $8b, $00, $81, $16
	.byte $41, $0c
	.word @song0ref2243
	.byte $41, $0c
	.word @song0ref2243
	.byte $41, $0b
	.word @song0ref1987
	.byte $22, $9d, $00, $81, $22, $8b, $00, $81, $1d, $8b, $00, $81, $16
	.byte $41, $0b
	.word @song0ref1804
	.byte $1b
	.byte $41, $0b
	.word @song0ref2151
@song0ref2284:
	.byte $19, $8b, $00, $81, $17, $af, $00, $81, $17, $8b, $00, $81, $17, $9d, $00, $81, $17, $8b, $00, $81, $17, $9d, $00, $81
	.byte $17
@song0ref2309:
	.byte $8b, $00, $81, $14, $8b, $00, $81, $14, $8b, $00, $81, $14
	.byte $41, $0c
	.word @song0ref2243
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0b
	.word @song0ref2151
	.byte $19
	.byte $41, $0b
	.word @song0ref1923
	.byte $19
	.byte $41, $0b
	.word @song0ref1915
	.byte $41, $0d
	.word @song0ref2011
	.byte $8b, $00, $81
	.byte $41, $25
	.word @song0ref2284
	.byte $41, $0c
	.word @song0ref2243
	.byte $41, $0c
	.word @song0ref1923
	.byte $41, $0b
	.word @song0ref2151
	.byte $19
	.byte $41, $0b
	.word @song0ref1923
	.byte $19, $8b, $00, $81, $19
	.byte $41, $0c
	.word @song0ref2309
	.byte $8b, $00, $81, $14
	.byte $41, $0c
	.word @song0ref2243
	.byte $8b, $00, $81, $16
	.byte $41, $0c
	.word @song0ref1923
	.byte $8b, $00, $81, $17
	.byte $41, $0b
	.word @song0ref2151
	.byte $19
	.byte $41, $0b
	.word @song0ref1915
	.byte $1b, $8b, $00, $93, $19, $8b, $00, $81, $19, $8b, $00, $93, $1b, $8b, $00, $81, $1b, $8b, $00, $93, $16
	.byte $41, $0c
	.word @song0ref2243
	.byte $8b, $00, $81, $16, $8b, $00, $81, $18, $8b, $00, $81, $1a, $8b, $00, $81, $42
	.word @song0ch2loop
@song0ch3:
@song0ch3loop:
@song0ref2436:
	.byte $86, $21, $a1, $80
@song0ref2440:
	.byte $1b, $a1, $86, $21, $a1, $80, $1b, $a1, $86, $21, $a1, $80, $1b, $a1
	.byte $41, $0c
	.word @song0ref2436
	.byte $41, $0a
	.word @song0ref2436
	.byte $21, $a1, $21, $a1, $21, $a1, $21, $a1, $21, $a1, $80, $1b, $8f, $86, $21, $8f, $80, $1b, $81, $1b, $8b, $86
@song0ref2482:
	.byte $21
@song0ref2483:
	.byte $8f, $82, $15, $8f, $84, $21, $8f, $80, $1b, $8f, $84, $21, $8f, $82, $15, $8f, $15, $8f, $80, $1b, $8f, $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
@song0ref2512:
	.byte $84, $21, $8f, $82, $15, $8f, $80, $1b, $8f, $1b, $8f, $82, $15, $8f, $80, $1b, $8f, $1b, $8f, $82, $15, $8f, $80, $1b
	.byte $41, $0f
	.word @song0ref2483
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $41, $11
	.word @song0ref2512
	.byte $41, $0f
	.word @song0ref2483
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84, $21, $8f, $80, $1b, $8f, $82, $15, $8f
@song0ref2586:
	.byte $15, $8f, $80, $1b, $8f, $82, $15, $8f, $80, $1b, $8f, $1b, $8f, $1b
	.byte $41, $0f
	.word @song0ref2483
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $41, $11
	.word @song0ref2512
	.byte $41, $0f
	.word @song0ref2483
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $41, $11
	.word @song0ref2512
	.byte $41, $0f
	.word @song0ref2483
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84
	.byte $41, $10
	.word @song0ref2482
	.byte $84, $21, $8f, $80, $1b, $8f, $1b, $8f, $82
	.byte $41, $0b
	.word @song0ref2586
	.byte $8f, $1b, $8f
@song0ref2670:
	.byte $1b, $a1, $1b, $8f, $1b, $a1, $1b, $8f, $1b, $a1, $82, $15, $8f, $80, $1b, $8f, $82
	.byte $41, $0a
	.word @song0ref2586
@song0ref2690:
	.byte $82, $15, $8f, $15, $8f, $86, $21, $8f, $82, $15, $8f, $15, $8f, $86, $21, $8f
	.byte $41, $0c
	.word @song0ref2690
	.byte $80, $1b, $a1
	.byte $41, $0a
	.word @song0ref2670
	.byte $1b, $8f, $1b, $a1, $82, $15, $8f, $80, $1b, $8f, $82
	.byte $41, $0a
	.word @song0ref2586
	.byte $41, $0c
	.word @song0ref2690
	.byte $41, $0c
	.word @song0ref2690
	.byte $80, $1b, $a1
	.byte $41, $0a
	.word @song0ref2440
	.byte $86, $21, $a1, $80, $1b, $a1, $86, $21, $a1, $80
	.byte $41, $0a
	.word @song0ref2670
	.byte $1b, $8f, $1b, $a1, $1b, $8f, $1b, $8f, $1b, $8f, $1b, $8f, $1b, $8f, $82, $15, $8f, $15, $8f, $42
	.word @song0ch3loop
@song0ch4:
@song0ch4loop:
@song0ref2777:
	.byte $ff, $ff, $9f, $ff, $ff, $9f, $ff, $ff, $9f, $ff, $ff, $9f
	.byte $41, $0c
	.word @song0ref2777
	.byte $41, $0c
	.word @song0ref2777
	.byte $41, $0c
	.word @song0ref2777
	.byte $41, $0c
	.word @song0ref2777
	.byte $41, $0c
	.word @song0ref2777
	.byte $ff, $ff, $9f, $42
	.word @song0ch4loop
