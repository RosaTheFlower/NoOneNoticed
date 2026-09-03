# NoOneNoticed | 🇧🇷🔛 🔝

Um monitor de sistema multiplataforma para terminal, escrito em C# (.NET).
Examina em tempo real todo o sistema de arquivos e processos em execução, registrando eventos importantes e alertando sobre uso excessivo de CPU/RAM.
Pressione **ESC** para encerrar a sessão e salvar o relatório, ou **SHIFT + ESC** para encerrar, salvar e abrir o relatório automaticamente.

## Funcionalidades

- **Monitoramento de arquivos** — rastreia criação, exclusão e renomeação de arquivos/pastas dentro de um diretório informado (incluindo subpastas)
- **Monitoramento de processos** — detecta processos sendo iniciados/encerrados e sinaliza uso alto de CPU ou RAM
- **Log em tempo real** — saída colorida no console, de acordo com o nível de severidade (`Info`, `Warning`, `Danger`, `Error`)
- **Exportação de relatório sob demanda** — grava todo o histórico da sessão em um arquivo `.txt` com timestamp, na pasta temporária do sistema (SHIFT+ESC para acessá-lo diretamente)

## Requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) (para compilar/rodar a partir do código-fonte)

## Download

- **[NoOneNoticed.exe](https://github.com/RosaTheFlower/NoOneNoticed/releases/download/v1.0.0/NoOneNoticed.exe)** (164 KB) — requer o [.NET Runtime](https://dotnet.microsoft.com/download) instalado
- **[NoOneNoticedPortable.exe](https://github.com/RosaTheFlower/NoOneNoticed/releases/download/v1.0.0/NoOneNoticedPortable.exe)** (64.4 MB) — standalone, não requer instalação

Veja todas as releases [aqui](https://github.com/RosaTheFlower/NoOneNoticed/releases/latest).

## Executando

```bash
dotnet run --project NoOneNoticed -- [opções]
```

Ou, após publicar, execute o binário diretamente:

```bash
NoOneNoticed.exe [opções]
```

### Opções

| Flag | Descrição | Padrão |
|------|-----------|--------|
| `--path <caminho>` | Diretório raiz a ser monitorado | `C:\` |
| `--range <ms>` | Intervalo de verificação de processos, em milissegundos | `2000` |
| `--ram <mb>` | Limiar de uso de RAM (MB) que dispara um aviso | `500` |

Exemplo:

```bash
NoOneNoticed.exe --path C:\Users --range 3000 --ram 300
```

### Controles

- **ESC** — encerra o monitoramento e exporta o relatório
- **SHIFT + ESC** — encerra o monitoramento, exporta o relatório e o abre automaticamente

O relatório é salvo na pasta temporária do sistema (`%TEMP%` no Windows), e o caminho é exibido no console assim que o monitoramento é encerrado.

## Publicando um executável standalone

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

Isso gera um único `.exe` que já inclui o runtime do .NET, podendo rodar em máquinas sem o .NET instalado.

## Observações

- Monitorar um disco inteiro (ex: `C:\`) pode gerar um volume muito alto de eventos de arquivo; o `FileSystemWatcher` interno pode ocasionalmente perder eventos sob carga pesada (registrado como um `Error` quando isso acontece). Para um rastreamento mais confiável, considere apontar o `--path` para uma pasta mais restrita.
- Ler alguns processos pertencentes ao sistema pode exigir execução do terminal como Administrador.

Nome inspirado em "No One Noticed", música do álbum *Submarine*, da banda The Marías.