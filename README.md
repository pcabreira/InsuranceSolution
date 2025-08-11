# InsuranceSolution - Arquitetura Hexagonal com .NET 8

Este projeto contempla 2 microserviços para criação e gerenciamento de propostas e contratos, implementado em **.NET 8**, utilizando **Arquitetura Hexagonal**, **MediatR**, **PostgreSQL** e **RabbitMQ**.

A arquitetura hexagonal (Ports and Adapters) garante baixo acoplamento e alta coesão, permitindo fácil manutenção, escalabilidade e testabilidade do código.

---

## 📌 Tecnologias Utilizadas

- **.NET 8**
- **C#**
- **MediatR** (CQRS)
- **Entity Framework Core** (PostgreSQL)
- **RabbitMQ** (Mensageria)
- **Docker Compose**
- **Swagger** (Documentação da API)

---

## 🏛 Arquitetura Hexagonal

A aplicação segue o padrão **Ports and Adapters**, dividindo o código em camadas:

- **Domain** → Entidades, regras de negócio, agregados e objetos de valor.
- **Application** → Casos de uso, comandos, consultas e manipuladores (handlers).
- **Infrastructure** → Persistência, repositórios, integração com RabbitMQ, contexto de banco de dados.
- **API** → Endpoints, controllers e configuração de injeção de dependências.
- **UnitTests** → Testes automatizados que verificam o funcionamento correto de pequenas partes isoladas do código, chamadas de unidades.
- **IntegrationTests** → Testes que verificam se diferentes partes do sistema funcionam bem juntas.

---

## 📂 Estrutura de Pastas

```

src/
├── ProposalService.API              # Camada de entrada (Controllers, Configurações, Swagger)
├── ProposalService.Application      # Casos de uso e Handlers
├── ProposalService.Domain           # Entidades e lógica de negócio
└── ProposalService.Infrastructure   # Persistência, Repositórios, RabbitMQ, Migrations
└── ProposalService.UnitTests        # Testes de unidade
└── ProposalService.IntegrationTests # Testes de Integração

---

## 🚀 Como Executar o Projeto

### 1️⃣ Clonar o Repositório

```bash
git clone https://github.com/pcabreira/InsuranceSolution.git
cd InsuranceSolution
```

### 2️⃣ Subir os Serviços com Docker

```bash
docker-compose up --build -d
```

Isso irá iniciar:

* **PostgreSQL** na porta `5432`
* **RabbitMQ** na porta `5672` (painel de gerenciamento em `http://localhost:15672`)

### 3️⃣ Criar e Aplicar Migrations

```bash
cd ProposalSolution
dotnet ef migrations add InitialCreate --project src/ProposalService.Infrastructure --startup-project src/ProposalService.API
dotnet ef database update --project src/ProposalService.Infrastructure --startup-project src/ProposalService.API
```

### 4️⃣ Rodar a Aplicação

```bash
dotnet run --project src/ProposalService.API
```

A API estará disponível em `http://localhost:5000/swagger`.

---

## 📬 Fluxo de Criação de Proposta

1. Enviar `POST /api/proposals` com JSON:

```json
{
  "customerId": "12345",
  "description": "Seguro de vida",
  "amount": 1500.50
}
```

2. O **Handler** cria a entidade `Proposal` no domínio.
3. Salva no PostgreSQL via `ProposalRepository`.
4. Publica mensagem no RabbitMQ quando ocorre aprovação.

---

## 🧪 Testando a API

Após rodar a aplicação, abra o Swagger:

```
http://localhost:5000/swagger
```

Execute os endpoints e verifique os logs no console e mensagens no RabbitMQ.

---

## 📄 Licença

Este projeto está sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para mais detalhes.

```

