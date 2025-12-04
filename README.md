
# Dago API

## Visão Geral
Esta API RESTful foi construída com ASP.NET Core (.NET 8) e disponibiliza endpoints para autenticação, gerenciamento de usuários, clientes, unidades, cargos e importação de dados. Ela espera requisições em JSON, utiliza autenticação JWT, e todas as rotas documentadas abaixo assumem o prefixo base:

```
https://{host}/api
```

## Autenticação

### Autenticar usuário
**POST** `/api/auth/login`  
Retorna um token JWT válido.

#### Corpo da requisição
```json
{
  "username": "string",
  "password": "string"
}
```

#### Resposta
```json
{
  "token": "string_jwt",
  "expiresIn": 3600
}
```

Use o token em rotas protegidas com:
```
Authorization: Bearer {token}
```

## Rotas de Usuário

### Listar usuários
**GET** `/api/users`  
Retorna lista paginada de usuários.

### Obter usuário
**GET** `/api/users/{id}`

### Criar usuário
**POST** `/api/users`
```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "role": "string"
}
```

### Atualizar usuário
**PUT** `/api/users/{id}`
```json
{
  "email": "string",
  "role": "string"
}
```

### Deletar usuário
**DELETE** `/api/users/{id}`

## Rotas de Clientes

### Listar clientes
**GET** `/api/clients`

### Obter cliente
**GET** `/api/clients/{id}`

### Criar cliente
**POST** `/api/clients`
```json
{
  "name": "string",
  "cnpj": "string"
}
```

### Atualizar cliente
**PUT** `/api/clients/{id}`

### Deletar cliente
**DELETE** `/api/clients/{id}`

## Unidades, Cargos
Rotas seguem o mesmo padrão de CRUD com endpoints:
- `/api/units`
- `/api/roles`

## Importação CTRC
**POST** `/api/import/ctrc`  
Importa arquivos com dados de CTRCs. Pode ser `multipart/form-data` ou JSON.

#### Resposta esperada
```json
{
  "imported": 123,
  "errors": [
    {
      "line": 5,
      "message": "CNPJ inválido"
    }
  ]
}
```

## Normalização de CNPJ
**POST** `/api/utils/normalize/cnpj`
```json
{
  "cnpj": "string"
}
```

#### Resposta
```json
{
  "normalizedCnpj": "string"
}
```

## Ambiente
`.env` com:
```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=dago
DB_USER=usuario
DB_PASSWORD=senha

JWT_KEY=chave
JWT_ISSUER=issuer
JWT_AUDIENCE=audience
```

## Rodando Localmente
```bash
dotnet ef database update
dotnet run
```

API estará em `https://localhost:5001`, com documentação interativa em `/swagger`.

## Observações
- CORS aceitando `localhost:3000` e `localhost:5173`
- Envie `Content-Type: application/json`
- Token obrigatório em rotas privadas

---

Para dúvidas ou sugestões, abra uma issue ou PR!
