import re
import spacy
import dateparser
from datetime import datetime, timedelta

nlp = spacy.load("es_core_news_sm")

# Diccionarios y listas base
PALABRAS_ACCION = [
    "recordar", "recordatorio", "alarma", "tomar", "hacer", "avisar", "notificar", "llamar", "escribir", "enviar",
    "comprar", "pagar", "revisar", "verificar", "preparar", "limpieza", "ir", "visitar", "completar", "terminar",
    "estudiar", "leer", "cocinar", "limpiar", "organizar", "compras", "compra", "pago", "reunion", "reunión",
    "cita", "tarea", "deber", "obligacion", "obligación", "compromiso"
]

PALABRAS_FUERTES = [
    "recordar", "recordatorio", "alarma", "recuérdame", "no olvides", "pon una alarma", "despiértame"
]

PALABRAS_TEMPORALES = [
    "hoy", "mañana", "tarde", "noche", "mediodía", "medianoche", "pasado mañana", "próximo", "siguiente",
    "esta", "este", "próxima"
]

PARTES_DIA_HORAS = {
    "mañana": "09:00",
    "tarde": "15:00",
    "noche": "20:00",
    "mediodía": "12:00",
    "medianoche": "00:00"
}

DIAS_SEMANA = {
    "lunes": 0, "martes": 1, "miércoles": 2, "jueves": 3,
    "viernes": 4, "sábado": 5, "domingo": 6
}

PATRONES_TEMPORALES = [
    r"hoy\b", r"mañana\b", r"pasado mañana\b",
    r"el (lunes|martes|miércoles|jueves|viernes|sábado|domingo)\b",
    r"a las \d{1,2}(?:[:.]?\d{2})?\s*(am|pm|a\.m\.|p\.m\.)?\b",
    r"en la (mañana|tarde|noche)\b", r"esta (mañana|tarde|noche|semana)\b",
    r"por la (mañana|tarde|noche)\b", r"al mediodía\b", r"a medianoche\b",
    r"\b\d{1,2}(?:[:.]?\d{2})?\s*(am|pm|a\.m\.|p\.m\.)?\b",
    r"\d{1,2}/\d{1,2}/\d{4}", r"\d{4}-\d{2}-\d{2}"
]

def analizar_texto(texto: str):
    texto = texto.strip().lower()
    doc = nlp(texto)
    ahora = datetime.now()

    entidades_temporales = []
    parte_dia_detectada = None
    hora_parte_dia = None
    fecha_detectada = None
    hora_manual = None
    horas_manual = 0
    minutos_manual = 0
    diagnostico = []

    # Acción
    tiene_accion = any(p in texto for p in PALABRAS_ACCION)
    acciones_spacy = [t.lemma_ for t in doc if t.pos_ in ["VERB", "NOUN"] and t.lemma_ in PALABRAS_ACCION]
    tiene_accion |= bool(acciones_spacy)
    if not tiene_accion:
        diagnostico.append("No se detectó acción relevante.")

    # Temporalidad
    tiene_temporal = any(p in texto for p in PALABRAS_TEMPORALES)
    if not tiene_temporal:
        diagnostico.append("No se detectó referencia temporal directa.")

    # Entidades spaCy
    for ent in doc.ents:
        if ent.label_ in ("DATE", "TIME"):
            entidades_temporales.append(ent.text)

    # Patrones temporales
    for patron in PATRONES_TEMPORALES:
        for match in re.finditer(patron, texto):
            if match.group() not in entidades_temporales:
                entidades_temporales.append(match.group())

    # Parte del día
    for parte, hora_defecto in PARTES_DIA_HORAS.items():
        if parte in texto:
            parte_dia_detectada = parte
            hora_parte_dia = hora_defecto
            if parte not in " ".join(entidades_temporales):
                entidades_temporales.append(parte)
            break

    # Hora explícita - CORREGIDO: Extraer valores correctamente
    patron_hora = re.search(
        r"(?:a las\s*)?(\d{1,2})(?:[:.]?(\d{2}))?\s*(am|pm|a\.m\.|p\.m\.)?\b", texto
    )
    if patron_hora:
        horas_manual = int(patron_hora.group(1))
        minutos_manual = int(patron_hora.group(2) or 0)
        meridiano = patron_hora.group(3)
        
        # Convertir a formato 24 horas
        if meridiano:
            if "p" in meridiano.lower() and horas_manual < 12:
                horas_manual += 12
            elif "a" in meridiano.lower() and horas_manual == 12:
                horas_manual = 0
        
        hora_manual = f"{horas_manual:02d}:{minutos_manual:02d}"

    # Resolución de fecha
    fecha_resuelta_manualmente = False
    
    if "pasado mañana" in texto:
        fecha_detectada = ahora + timedelta(days=2)
        fecha_resuelta_manualmente = True
    elif "mañana" in texto:
        fecha_detectada = ahora + timedelta(days=1)
        fecha_resuelta_manualmente = True
    else:
        # Detectar días de la semana
        for dia_texto, dia_num in DIAS_SEMANA.items():
            if f"el {dia_texto}" in texto or f"este {dia_texto}" in texto or f"siguiente {dia_texto}" in texto:
                hoy_num = ahora.weekday()
                delta = (dia_num - hoy_num + 7) % 7
                if delta == 0:  # Si es hoy, usar la próxima semana
                    delta = 7
                fecha_detectada = ahora + timedelta(days=delta)
                fecha_resuelta_manualmente = True
                break

    # Parseo con dateparser - CORREGIDO: Mejor configuración para fechas en español
    if not fecha_detectada:
        for ent in entidades_temporales:
            fecha_parseada = dateparser.parse(
                ent, 
                languages=['es'], 
                settings={
                    'DATE_ORDER': 'DMY',
                    'PREFER_DAY_OF_MONTH': 'first',
                    'PREFER_DATES_FROM': 'future'
                }
            )
            if fecha_parseada:
                fecha_detectada = fecha_parseada
                break
        
        # Si no se detectó con entidades, intentar parsear el texto completo
        if not fecha_detectada:
            fecha_detectada = dateparser.parse(
                texto, 
                languages=['es'], 
                settings={
                    'DATE_ORDER': 'DMY',
                    'PREFER_DAY_OF_MONTH': 'first',
                    'PREFER_DATES_FROM': 'future'
                }
            )

    # CORRECCIÓN CRÍTICA: Aplicar hora manual SIEMPRE que exista
    if fecha_detectada and hora_manual:
        # Si tenemos hora manual, aplicarla a la fecha detectada
        fecha_detectada = fecha_detectada.replace(
            hour=horas_manual, 
            minute=minutos_manual, 
            second=0, 
            microsecond=0
        )
    elif fecha_detectada and parte_dia_detectada and hora_parte_dia:
        # Si tenemos parte del día pero no hora manual
        hora_partes = hora_parte_dia.split(':')
        fecha_detectada = fecha_detectada.replace(
            hour=int(hora_partes[0]), 
            minute=int(hora_partes[1]), 
            second=0, 
            microsecond=0
        )
    elif fecha_detectada and not fecha_resuelta_manualmente:
        # Si la fecha viene de dateparser pero no tiene hora, usar hora por defecto (09:00)
        if fecha_detectada.hour == 0 and fecha_detectada.minute == 0:
            fecha_detectada = fecha_detectada.replace(hour=9, minute=0, second=0, microsecond=0)

    # Componentes finales
    dia = fecha_detectada.strftime("%d") if fecha_detectada else None
    mes = fecha_detectada.strftime("%m") if fecha_detectada else None
    hora = fecha_detectada.strftime("%H:%M") if fecha_detectada else None
    fecha_completa = fecha_detectada.isoformat() if fecha_detectada else None

    # Intención de recordatorio
    tiene_palabra_fuerte = any(p in texto for p in PALABRAS_FUERTES)
    tiene_fecha_hora = bool(fecha_detectada or hora_manual or parte_dia_detectada)
    detecto_recordatorio = (
        tiene_palabra_fuerte or
        (tiene_accion and tiene_temporal) or
        (tiene_accion and tiene_fecha_hora) or
        (tiene_temporal and bool(acciones_spacy))
    )
    if not detecto_recordatorio:
        diagnostico.append("No se detectó intención clara de recordatorio.")

    if not fecha_detectada:
        diagnostico.append("No se pudo resolver una fecha válida.")
    if fecha_detectada and not hora:
        diagnostico.append("No se detectó hora explícita ni parte del día.")

    return {
        "dia": dia,
        "mes": mes,
        "hora": hora,
        "fecha_completa": fecha_completa,
        "detecto_recordatorio": detecto_recordatorio,
        "texto_original": texto,
        "entidades_detectadas": entidades_temporales,
        "diagnostico": diagnostico
    }