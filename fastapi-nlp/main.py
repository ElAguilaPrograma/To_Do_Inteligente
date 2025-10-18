from fastapi import FastAPI
from pydantic import BaseModel
from fastapi.middleware.cors import CORSMiddleware
from nlp_service.processor import analizar_texto

app = FastAPI(
    title="API NLP Para Recordatorios",
    description="Servicio inteligente para analizar cadenas de texto naturales y extraer recordatorios usando spaCy y dateparser",
    version="2.0.0"
    )

# Permitir peticiones desde ASP.NET Core
app.add_middleware(
    CORSMiddleware,
    allow_origins=["https://localhost:7035"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
    )

class TextoEntrada(BaseModel):
    texto: str

@app.post("/analizar")
def analizar_texto_endpoint(data: TextoEntrada):
    resultado = analizar_texto(data.texto)
    return resultado