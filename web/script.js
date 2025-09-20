// IPaaS Expression Builder - Main JavaScript Implementation

class ExpressionBuilder {
    constructor() {
        this.functions = {
            ADD: (...args) => args.reduce((sum, val) => sum + val, 0),
            SUBTRACT: (a, b) => a - b,
            MULTIPLY: (...args) => args.reduce((product, val) => product * val, 1),
            DIVIDE: (a, b) => {
                if (b === 0) throw new Error('Division by zero');
                return a / b;
            },
            MOD: (a, b) => ((Number(a) % Number(b)) + Number(b)) % Number(b),
            POW: (a, b) => Math.pow(Number(a), Number(b)),
            ROUND: (a, digits = 0) => {
                const factor = Math.pow(10, Number(digits));
                return Math.round(Number(a) * factor) / factor;
            },
            MAX: (...args) => Math.max(...args.map(Number)),
            MIN: (...args) => Math.min(...args.map(Number)),
            CONCAT: (...args) => args.join(''),
            JOIN: (arr, sep = '') => Array.isArray(arr) ? arr.join(String(sep)) : String(arr),
            SPLIT: (str, sep) => String(str).split(String(sep)),
            REPLACE: (str, search, replace) => String(str).split(String(search)).join(String(replace)),
            INDEXOF: (str, search) => String(str).indexOf(String(search)),
            EQUALS: (a, b) => a === b,
            NOT_EQUALS: (a, b) => a !== b,
            GREATER: (a, b) => a > b,
            LESS: (a, b) => a < b,
            GREATER_EQUAL: (a, b) => a >= b,
            LESS_EQUAL: (a, b) => a <= b,
            IF: (condition, trueVal, falseVal) => condition ? trueVal : falseVal,
            AND: (...args) => args.every(Boolean),
            OR: (...args) => args.some(Boolean),
            NOT: (val) => !val,
            LENGTH: (str) => str.length,
            UPPER: (str) => str.toUpperCase(),
            LOWER: (str) => str.toLowerCase(),
            TRIM: (str) => str.trim(),
            SUBSTRING: (str, start, end) => str.substring(start, end),
            CONTAINS: (str, substr) => str.includes(substr),
            STARTS_WITH: (str, prefix) => str.startsWith(prefix),
            ENDS_WITH: (str, suffix) => str.endsWith(suffix),
            INCLUDES: (container, item) => Array.isArray(container) ? container.includes(item) : String(container).includes(String(item)),
            STARTSWITH: (str, prefix) => String(str).startsWith(String(prefix)),
            ENDSWITH: (str, suffix) => String(str).endsWith(String(suffix)),
            FIRST: (arr) => Array.isArray(arr) ? arr[0] : undefined,
            LAST: (arr) => Array.isArray(arr) ? arr[arr.length - 1] : undefined,
            COUNT: (arr) => Array.isArray(arr) ? arr.length : (typeof arr === 'string' ? arr.length : 0),
            SKIP: (arr, n) => Array.isArray(arr) ? arr.slice(Number(n)) : arr,
            TAKE: (arr, n) => Array.isArray(arr) ? arr.slice(0, Number(n)) : arr,
            APPEND: (arr, item) => Array.isArray(arr) ? [...arr, item] : [arr, item],
            INTERSECTION: (a, b) => Array.isArray(a) && Array.isArray(b) ? a.filter(x => b.includes(x)) : [],
            UNION: (a, b) => Array.isArray(a) && Array.isArray(b) ? Array.from(new Set([...a, ...b])) : [],
            SUMBYPROPERTY: (arr, prop) => Array.isArray(arr) ? arr.reduce((s, it) => s + Number(it?.[prop] ?? 0), 0) : 0,
            CONCATBYPROPERTY: (arr, prop, sep = '') => Array.isArray(arr) ? arr.map(it => it?.[prop]).join(String(sep)) : '',
            EMPTY: (val) => val == null || (typeof val === 'string' && val.length === 0) || (Array.isArray(val) && val.length === 0) || (typeof val === 'object' && Object.keys(val).length === 0),
            BETWEEN: (val, min, max, inclusive = true) => inclusive ? (val >= min && val <= max) : (val > min && val < max),
            ADDDAYS: (date, days) => new Date(Date.parse(date) + Number(days) * 86400000).toISOString(),
            ADDHOURS: (date, hours) => new Date(Date.parse(date) + Number(hours) * 3600000).toISOString(),
            ADDMINUTES: (date, minutes) => new Date(Date.parse(date) + Number(minutes) * 60000).toISOString(),
            ADDSECONDS: (date, seconds) => new Date(Date.parse(date) + Number(seconds) * 1000).toISOString(),
            FORMATDATETIME: (date, fmt) => {
                const d = new Date(Date.parse(date));
                // Very small formatter supporting a few tokens
                const pad = (n, l=2) => String(n).padStart(l, '0');
                return String(fmt)
                    .replace(/yyyy/g, d.getUTCFullYear())
                    .replace(/MM/g, pad(d.getUTCMonth()+1))
                    .replace(/dd/g, pad(d.getUTCDate()))
                    .replace(/HH/g, pad(d.getUTCHours()))
                    .replace(/mm/g, pad(d.getUTCMinutes()))
                    .replace(/ss/g, pad(d.getUTCSeconds()));
            },
            UTCNOW: () => new Date().toISOString(),
            TODAY: () => {
                const d = new Date();
                return new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate())).toISOString();
            },
            DAYOFWEEK: (date) => new Date(Date.parse(date)).getUTCDay(),
            DAYOFMONTH: (date) => new Date(Date.parse(date)).getUTCDate(),
            DAYOFYEAR: (date) => {
                const d = new Date(Date.parse(date));
                const start = new Date(Date.UTC(d.getUTCFullYear(),0,1));
                return Math.floor((d - start) / 86400000) + 1;
            },
            DATEDIFF: (a, b, unit = 'milliseconds') => {
                const ms = Date.parse(b) - Date.parse(a);
                switch (String(unit)) {
                    case 'seconds': return Math.floor(ms/1000);
                    case 'minutes': return Math.floor(ms/60000);
                    case 'hours': return Math.floor(ms/3600000);
                    case 'days': return Math.floor(ms/86400000);
                    default: return ms;
                }
            },
            TICKS: (date) => {
                const ms = Date.parse(date);
                const epochOffsetMs = 62135596800000; // .NET epoch offset
                return (ms + epochOffsetMs) * 10000;
            },
            BASE64: (str) => typeof btoa !== 'undefined' ? btoa(String(str)) : Buffer.from(String(str), 'utf8').toString('base64'),
            BASE64TOSTRING: (b64) => typeof atob !== 'undefined' ? atob(String(b64)) : Buffer.from(String(b64), 'base64').toString('utf8'),
            SETJSONPROPERTY: (obj, key, value) => {
                if (obj == null || typeof obj !== 'object') throw new Error('SETJSONPROPERTY: first argument must be an object');
                const copy = Array.isArray(obj) ? obj.slice() : { ...obj };
                copy[String(key)] = value;
                return copy;
            },
            GETXMLPROPERTY: (xml, tag) => {
                const parser = new DOMParser();
                const doc = parser.parseFromString(String(xml), 'application/xml');
                const el = doc.getElementsByTagName(String(tag))[0];
                return el ? el.textContent : null;
            },
            SETXMLPROPERTY: (xml, tag, value) => {
                const parser = new DOMParser();
                const serializer = new XMLSerializer();
                const doc = parser.parseFromString(String(xml), 'application/xml');
                let el = doc.getElementsByTagName(String(tag))[0];
                if (!el) {
                    el = doc.createElement(String(tag));
                    const root = doc.documentElement || doc.appendChild(doc.createElement('root'));
                    root.appendChild(el);
                }
                el.textContent = value != null ? String(value) : '';
                return serializer.serializeToString(doc);
            }
        };

        // JSON & Collections
        this.functions.GETJSONPROPERTY = (obj, key) => {
            if (obj == null || typeof obj !== 'object') throw new Error('GETJSONPROPERTY: first argument must be an object');
            return obj[key];
        };
        this.functions.FIRST = (arr) => {
            if (!Array.isArray(arr)) throw new Error('FIRST: argument must be an array');
            return arr[0];
        };

        // Function catalog metadata for DevExtreme lists
        this.functionCatalog = [
            {
                group: 'Mathematical',
                items: [
                    { name: 'ADD', signature: 'ADD(a, b, ...)', description: 'Adds numbers together', example: '[ADD(1,2,3)] → 6' },
                    { name: 'SUBTRACT', signature: 'SUBTRACT(a, b)', description: 'Subtracts b from a', example: '[SUBTRACT(5,2)] → 3' },
                    { name: 'MULTIPLY', signature: 'MULTIPLY(a, b, ...)', description: 'Multiplies numbers', example: '[MULTIPLY(2,3,4)] → 24' },
                    { name: 'DIVIDE', signature: 'DIVIDE(a, b)', description: 'Divides a by b', example: '[DIVIDE(10,2)] → 5' }
                ]
            },
            {
                group: 'JSON',
                items: [
                    { name: 'GETJSONPROPERTY', signature: 'GETJSONPROPERTY(obj, key)', description: 'Gets a property from an object literal by key', example: '[GETJSONPROPERTY({"Name":"Test"}, "Name")] → "Test"' }
                ]
            },
            {
                group: 'Collections',
                items: [
                    { name: 'FIRST', signature: 'FIRST(array)', description: 'Returns the first element of an array', example: '[FIRST([1,2,3])] → 1' }
                ]
            },
            {
                group: 'Strings',
                items: [
                    { name: 'CONCAT', signature: 'CONCAT(str1, str2, ...)', description: 'Concatenates strings', example: '[CONCAT("Hello"," ","World")] → "Hello World"' },
                    { name: 'LENGTH', signature: 'LENGTH(str)', description: 'Returns string length', example: '[LENGTH("abc")] → 3' },
                    { name: 'UPPER', signature: 'UPPER(str)', description: 'Converts to uppercase', example: '[UPPER("abc")] → "ABC"' },
                    { name: 'LOWER', signature: 'LOWER(str)', description: 'Converts to lowercase', example: '[LOWER("ABC")] → "abc"' },
                    { name: 'TRIM', signature: 'TRIM(str)', description: 'Trims whitespace', example: '[TRIM("  a  ")] → "a"' },
                    { name: 'SUBSTRING', signature: 'SUBSTRING(str, start, end)', description: 'Extracts substring', example: '[SUBSTRING("Hello", 1, 4)] → "ell"' },
                    { name: 'CONTAINS', signature: 'CONTAINS(str, substr)', description: 'Checks substring presence', example: '[CONTAINS("abc","b")] → true' },
                    { name: 'STARTS_WITH', signature: 'STARTS_WITH(str, prefix)', description: 'Checks prefix', example: '[STARTS_WITH("abc","a")] → true' },
                    { name: 'ENDS_WITH', signature: 'ENDS_WITH(str, suffix)', description: 'Checks suffix', example: '[ENDS_WITH("abc","c")] → true' }
                ]
            },
            {
                group: 'Comparison',
                items: [
                    { name: 'EQUALS', signature: 'EQUALS(a, b)', description: 'Checks equality (strict)', example: '[EQUALS(1,1)] → true' },
                    { name: 'NOT_EQUALS', signature: 'NOT_EQUALS(a, b)', description: 'Checks inequality (strict)', example: '[NOT_EQUALS(1,2)] → true' },
                    { name: 'GREATER', signature: 'GREATER(a, b)', description: 'a > b', example: '[GREATER(3,2)] → true' },
                    { name: 'LESS', signature: 'LESS(a, b)', description: 'a < b', example: '[LESS(1,2)] → true' },
                    { name: 'GREATER_EQUAL', signature: 'GREATER_EQUAL(a, b)', description: 'a ≥ b', example: '[GREATER_EQUAL(2,2)] → true' },
                    { name: 'LESS_EQUAL', signature: 'LESS_EQUAL(a, b)', description: 'a ≤ b', example: '[LESS_EQUAL(2,2)] → true' }
                ]
            },
            {
                group: 'Logical',
                items: [
                    { name: 'IF', signature: 'IF(condition, trueVal, falseVal)', description: 'Conditional expression', example: '[IF(true, "Yes", "No")] → "Yes"' },
                    { name: 'AND', signature: 'AND(...args)', description: 'Logical AND over all args', example: '[AND(true, true, false)] → false' },
                    { name: 'OR', signature: 'OR(...args)', description: 'Logical OR over all args', example: '[OR(false, true)] → true' },
                    { name: 'NOT', signature: 'NOT(val)', description: 'Logical negation', example: '[NOT(false)] → true' }
                ]
            }
        ];

        this.initializeEventListeners();
    }

    // Token types
    static TOKEN_TYPES = {
        LEFT_BRACKET: 'LEFT_BRACKET',
        RIGHT_BRACKET: 'RIGHT_BRACKET',
        LEFT_PAREN: 'LEFT_PAREN',
        RIGHT_PAREN: 'RIGHT_PAREN',
        COMMA: 'COMMA',
        FUNCTION: 'FUNCTION',
        IDENTIFIER: 'IDENTIFIER',
        AT: 'AT',
        LEFT_BRACE: 'LEFT_BRACE',
        RIGHT_BRACE: 'RIGHT_BRACE',
        COLON: 'COLON',
        STRING: 'STRING',
        NUMBER: 'NUMBER',
        BOOLEAN: 'BOOLEAN',
        WHITESPACE: 'WHITESPACE',
        EOF: 'EOF'
    };

    // Lexical Analyzer
    tokenize(input) {
        const tokens = [];
        let position = 0;
        const inputLength = input.length;

        while (position < inputLength) {
            const char = input[position];
            
            // Skip whitespace
            if (/\s/.test(char)) {
                position++;
                continue;
            }

            // Left bracket [
            if (char === '[') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.LEFT_BRACKET, value: '[', position });
                position++;
            }
            // Right bracket ]
            else if (char === ']') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.RIGHT_BRACKET, value: ']', position });
                position++;
            }
            // Left parenthesis (
            else if (char === '(') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.LEFT_PAREN, value: '(', position });
                position++;
            }
            // Right parenthesis )
            else if (char === ')') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.RIGHT_PAREN, value: ')', position });
                position++;
            }
            // Comma
            else if (char === ',') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.COMMA, value: ',', position });
                position++;
            }
            // Braces and colon for objects
            else if (char === '{') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.LEFT_BRACE, value: '{', position });
                position++;
            }
            else if (char === '}') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.RIGHT_BRACE, value: '}', position });
                position++;
            }
            else if (char === ':') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.COLON, value: ':', position });
                position++;
            }
            // Variable at-sign
            else if (char === '@') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.AT, value: '@', position });
                position++;
            }
            // String literal
            else if (char === '"' || char === "'") {
                const quote = char;
                let value = '';
                const startPosition = position + 1; // start after opening quote
                position++; // Skip opening quote
                
                while (position < inputLength && input[position] !== quote) {
                    if (input[position] === '\\' && position + 1 < inputLength) {
                        // Handle escape sequences
                        position++;
                        const nextChar = input[position];
                        switch (nextChar) {
                            case 'n': value += '\n'; break;
                            case 't': value += '\t'; break;
                            case 'r': value += '\r'; break;
                            case '\\': value += '\\'; break;
                            case '"': value += '"'; break;
                            case "'": value += "'"; break;
                            default: value += nextChar; break;
                        }
                    } else {
                        value += input[position];
                    }
                    position++;
                }
                
                if (position >= inputLength) {
                    throw new Error(`Unterminated string literal at position ${position}`);
                }
                
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.STRING, value, position: startPosition });
                position++; // Skip closing quote
            }
            // Number literal
            else if (/\d/.test(char)) {
                let value = '';
                const startPosition = position;
                while (position < inputLength && (/\d/.test(input[position]) || input[position] === '.')) {
                    value += input[position];
                    position++;
                }
                const numValue = value.includes('.') ? parseFloat(value) : parseInt(value, 10);
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.NUMBER, value: numValue, position: startPosition });
            }
            // Boolean literal
            else if (char === 't' && input.substr(position, 4) === 'true') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.BOOLEAN, value: true, position });
                position += 4;
            }
            else if (char === 'f' && input.substr(position, 5) === 'false') {
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.BOOLEAN, value: false, position });
                position += 5;
            }
            // Identifier (variables, names): [A-Za-z_][A-Za-z0-9_]*
            else if (/[A-Za-z_]/.test(char)) {
                let value = '';
                const startPosition = position;
                while (position < inputLength && /[A-Za-z0-9_]/.test(input[position])) {
                    value += input[position];
                    position++;
                }
                // Uppercase => function; else identifier
                if (/^[A-Z0-9_]+$/.test(value)) {
                    tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.FUNCTION, value, position: startPosition });
                } else {
                    tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.IDENTIFIER, value, position: startPosition });
                }
            }
            // Function name
            else if (/[A-Z_]/.test(char)) {
                let value = '';
                const startPosition = position;
                while (position < inputLength && /[A-Z_0-9]/.test(input[position])) {
                    value += input[position];
                    position++;
                }
                tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.FUNCTION, value, position: startPosition });
            }
            else {
                throw new Error(`Unexpected character '${char}' at position ${position}`);
            }
        }

        tokens.push({ type: ExpressionBuilder.TOKEN_TYPES.EOF, value: null, position });
        return tokens;
    }

    // Parser
    parse(tokens) {
        let current = 0;

        const peek = () => tokens[current];
        const advance = () => tokens[current++];
        const match = (type) => {
            if (peek() && peek().type === type) {
                return advance();
            }
            return null;
        };

        const parseExpression = () => {
            const leftBracket = match(ExpressionBuilder.TOKEN_TYPES.LEFT_BRACKET);
            if (!leftBracket) {
                throw new Error('Expected [ at the beginning of expression');
            }

            // Variable reference: [@identifier]
            if (peek().type === ExpressionBuilder.TOKEN_TYPES.AT) {
                advance(); // consume '@'
                const ident = match(ExpressionBuilder.TOKEN_TYPES.IDENTIFIER);
                if (!ident) {
                    throw new Error('Expected identifier after @');
                }
                const rightBracketVar = match(ExpressionBuilder.TOKEN_TYPES.RIGHT_BRACKET);
                if (!rightBracketVar) {
                    throw new Error('Expected ] at the end of variable reference');
                }
                return { type: 'variable', name: ident.value };
            }

            const functionToken = match(ExpressionBuilder.TOKEN_TYPES.FUNCTION);
            if (!functionToken) {
                throw new Error('Expected function name or @variable after [');
            }

            const leftParen = match(ExpressionBuilder.TOKEN_TYPES.LEFT_PAREN);
            if (!leftParen) {
                throw new Error('Expected ( after function name');
            }

            const args = parseArguments();

            const rightParen = match(ExpressionBuilder.TOKEN_TYPES.RIGHT_PAREN);
            if (!rightParen) {
                throw new Error('Expected ) after function arguments');
            }

            const rightBracket = match(ExpressionBuilder.TOKEN_TYPES.RIGHT_BRACKET);
            if (!rightBracket) {
                throw new Error('Expected ] at the end of expression');
            }

            return {
                type: 'function_call',
                function: functionToken.value,
                arguments: args
            };
        };

        const parseArguments = () => {
            const args = [];
            
            // Handle empty arguments
            if (peek() && peek().type === ExpressionBuilder.TOKEN_TYPES.RIGHT_PAREN) {
                return args;
            }

            while (true) {
                const arg = parseArgument();
                args.push(arg);

                if (match(ExpressionBuilder.TOKEN_TYPES.COMMA)) {
                    continue;
                } else if (peek() && peek().type === ExpressionBuilder.TOKEN_TYPES.RIGHT_PAREN) {
                    break;
                } else {
                    throw new Error('Expected , or ) after argument');
                }
            }

            return args;
        };

        const parseArgument = () => {
            const token = peek();
            
            if (!token) {
                throw new Error('Unexpected end of input');
            }

            switch (token.type) {
                case ExpressionBuilder.TOKEN_TYPES.STRING:
                case ExpressionBuilder.TOKEN_TYPES.NUMBER:
                case ExpressionBuilder.TOKEN_TYPES.BOOLEAN:
                    return advance();
                case ExpressionBuilder.TOKEN_TYPES.LEFT_BRACE:
                    return parseObjectLiteral();
                case ExpressionBuilder.TOKEN_TYPES.LEFT_BRACKET:
                    // Disambiguate: array literal vs nested expression
                    const next = tokens[current + 1];
                    if (next && (next.type === ExpressionBuilder.TOKEN_TYPES.FUNCTION || next.type === ExpressionBuilder.TOKEN_TYPES.AT)) {
                        return parseExpression();
                    }
                    return parseArrayLiteral();
                default:
                    throw new Error(`Unexpected token type: ${token.type}`);
            }
        };

        const parseObjectLiteral = () => {
            // very small JSON subset: { "key": value, ... }
            match(ExpressionBuilder.TOKEN_TYPES.LEFT_BRACE);
            const properties = {};
            if (peek().type === ExpressionBuilder.TOKEN_TYPES.RIGHT_BRACE) {
                advance();
                return { type: 'object_literal', value: properties };
            }
            while (true) {
                const keyTok = match(ExpressionBuilder.TOKEN_TYPES.STRING) || match(ExpressionBuilder.TOKEN_TYPES.IDENTIFIER);
                if (!keyTok) throw new Error('Expected property name');
                if (!match(ExpressionBuilder.TOKEN_TYPES.COLON)) throw new Error('Expected : after property name');
                const valueTok = parseValueForObject();
                properties[keyTok.value] = valueTok.type === 'function_call' || valueTok.type === 'object_literal' ? valueTok : valueTok.value;
                if (match(ExpressionBuilder.TOKEN_TYPES.COMMA)) continue;
                if (peek().type === ExpressionBuilder.TOKEN_TYPES.RIGHT_BRACE) { advance(); break; }
                throw new Error('Expected , or } after property');
            }
            return { type: 'object_literal', value: properties };
        };

        const parseValueForObject = () => {
            const t = peek();
            switch (t.type) {
                case ExpressionBuilder.TOKEN_TYPES.STRING:
                case ExpressionBuilder.TOKEN_TYPES.NUMBER:
                case ExpressionBuilder.TOKEN_TYPES.BOOLEAN:
                    return advance();
                case ExpressionBuilder.TOKEN_TYPES.LEFT_BRACE:
                    return parseObjectLiteral();
                case ExpressionBuilder.TOKEN_TYPES.LEFT_BRACKET:
                    // Could be array literal or nested expr
                    const next = tokens[current + 1];
                    if (next && (next.type === ExpressionBuilder.TOKEN_TYPES.FUNCTION || next.type === ExpressionBuilder.TOKEN_TYPES.AT)) {
                        return parseExpression();
                    }
                    return parseArrayLiteral();
                default:
                    throw new Error('Unsupported value in object literal');
            }
        };

        const parseArrayLiteral = () => {
            match(ExpressionBuilder.TOKEN_TYPES.LEFT_BRACKET);
            const elements = [];
            if (peek().type === ExpressionBuilder.TOKEN_TYPES.RIGHT_BRACKET) {
                advance();
                return { type: 'array_literal', value: elements };
            }
            while (true) {
                const t = peek();
                let elem;
                switch (t.type) {
                    case ExpressionBuilder.TOKEN_TYPES.STRING:
                    case ExpressionBuilder.TOKEN_TYPES.NUMBER:
                    case ExpressionBuilder.TOKEN_TYPES.BOOLEAN:
                        elem = advance();
                        break;
                    case ExpressionBuilder.TOKEN_TYPES.LEFT_BRACE:
                        elem = parseObjectLiteral();
                        break;
                    case ExpressionBuilder.TOKEN_TYPES.LEFT_BRACKET: {
                        const la = tokens[current + 1];
                        if (la && (la.type === ExpressionBuilder.TOKEN_TYPES.FUNCTION || la.type === ExpressionBuilder.TOKEN_TYPES.AT)) {
                            elem = parseExpression();
                        } else {
                            elem = parseArrayLiteral();
                        }
                        break;
                    }
                    default:
                        throw new Error('Unsupported array element');
                }
                elements.push(elem);
                if (match(ExpressionBuilder.TOKEN_TYPES.COMMA)) continue;
                if (peek().type === ExpressionBuilder.TOKEN_TYPES.RIGHT_BRACKET) { advance(); break; }
                throw new Error('Expected , or ] after array element');
            }
            return { type: 'array_literal', value: elements };
        };

        const ast = parseExpression();
        
        // Ensure we've consumed all tokens
        if (peek() && peek().type !== ExpressionBuilder.TOKEN_TYPES.EOF) {
            throw new Error('Unexpected tokens after expression');
        }

        return ast;
    }

    // Evaluator
    evaluate(ast) {
        if (ast.type === 'function_call') {
            const func = this.functions[ast.function];
            if (!func) {
                throw new Error(`Unknown function: ${ast.function}`);
            }

            const evaluatedArgs = ast.arguments.map(arg => {
                if (arg.type === 'function_call') {
                    return this.evaluate(arg);
                } else if (arg.type === 'object_literal') {
                    const obj = {};
                    for (const [k, v] of Object.entries(arg.value)) {
                        if (v && typeof v === 'object' && (v.type === 'function_call' || v.type === 'object_literal')) {
                            obj[k] = this.evaluate(v);
                        } else {
                            obj[k] = v;
                        }
                    }
                    return obj;
                } else if (arg.type === 'array_literal') {
                    return arg.value.map(el => {
                        if (el && typeof el === 'object' && (el.type === 'function_call' || el.type === 'object_literal' || el.type === 'array_literal')) {
                            return this.evaluate(el);
                        }
                        return el.value;
                    });
                } else {
                    return arg.value;
                }
            });

            return func(...evaluatedArgs);
        } else if (ast.type === 'variable') {
            if (!this.variables || !(ast.name in this.variables)) {
                throw new Error(`Variable not defined: ${ast.name}`);
            }
            return this.variables[ast.name];
        }

        throw new Error('Invalid AST node');
    }

    // Variable resolution
    setVariable(name, value) {
        if (!this.variables) this.variables = {};
        this.variables[name] = value;
    }

    getVariable(name) {
        if (!this.variables || !(name in this.variables)) {
            throw new Error(`Variable not defined: ${name}`);
        }
        return this.variables[name];
    }

    // Syntax highlighting
    applySyntaxHighlighting(text) {
        const tokens = this.tokenize(text);
        let highlighted = '';
        let lastPosition = 0;

        for (const token of tokens) {
            if (token.type === ExpressionBuilder.TOKEN_TYPES.EOF) break;
            
            const start = token.position;
            const end = start + (token.value === null ? 0 : token.value.toString().length);
            
            // Add any text before this token
            if (start > lastPosition) {
                highlighted += text.substring(lastPosition, start);
            }

            // Add highlighted token
            let className = '';
            switch (token.type) {
                case ExpressionBuilder.TOKEN_TYPES.FUNCTION:
                    className = 'syntax-function';
                    break;
                case ExpressionBuilder.TOKEN_TYPES.LEFT_BRACKET:
                case ExpressionBuilder.TOKEN_TYPES.RIGHT_BRACKET:
                case ExpressionBuilder.TOKEN_TYPES.LEFT_PAREN:
                case ExpressionBuilder.TOKEN_TYPES.RIGHT_PAREN:
                    className = 'syntax-bracket';
                    break;
                case ExpressionBuilder.TOKEN_TYPES.STRING:
                    className = 'syntax-string';
                    break;
                case ExpressionBuilder.TOKEN_TYPES.NUMBER:
                    className = 'syntax-number';
                    break;
                case ExpressionBuilder.TOKEN_TYPES.COMMA:
                    className = 'syntax-comma';
                    break;
            }

            if (className) {
                highlighted += `<span class="${className}">${token.value}</span>`;
            } else {
                highlighted += token.value;
            }

            lastPosition = end;
        }

        // Add any remaining text
        if (lastPosition < text.length) {
            highlighted += text.substring(lastPosition);
        }

        return highlighted;
    }

    // Main execution method
    execute(expression) {
        try {
            const tokens = this.tokenize(expression);
            const ast = this.parse(tokens);
            const result = this.evaluate(ast);
            
            return {
                success: true,
                result: result,
                tokens: tokens,
                ast: ast,
                steps: this.getExecutionSteps(ast)
            };
        } catch (error) {
            return {
                success: false,
                error: error.message,
                tokens: null,
                ast: null,
                steps: null
            };
        }
    }

    getExecutionSteps(ast) {
        const steps = [];
        
        const traverse = (node, depth = 0) => {
            if (node.type === 'function_call') {
                steps.push({
                    depth,
                    action: `Calling function: ${node.function}`,
                    args: node.arguments.map(arg => 
                        arg.type === 'function_call' ? `[${arg.function}(...)]` : arg.value
                    )
                });
                
                node.arguments.forEach(arg => {
                    if (arg.type === 'function_call') {
                        traverse(arg, depth + 1);
                    }
                });
            }
        };

        traverse(ast);
        return steps;
    }

    // Event listeners
    initializeEventListeners() {
        const expressionInput = document.getElementById('expressionInput');
        const compileBtn = document.getElementById('compileBtn');
        const clearBtn = document.getElementById('clearBtn');
        const syntaxOverlay = document.getElementById('syntaxOverlay');
        const resultOutput = document.getElementById('resultOutput');
        const tokensOutput = document.getElementById('tokensOutput');
        const astOutput = document.getElementById('astOutput');
        const stepsOutput = document.getElementById('stepsOutput');
        const groupListEl = document.getElementById('groupList');
        const functionListEl = document.getElementById('functionList');
        const functionDescriptionEl = document.getElementById('functionDescription');
        const varNameInput = document.getElementById('varName');
        const varValueInput = document.getElementById('varValue');
        const addVarBtnEl = document.getElementById('addVarBtn');
        const variablesListEl = document.getElementById('variablesList');
        const self = this;

        // Example buttons
        document.querySelectorAll('.example-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const expr = btn.getAttribute('data-expr');
                expressionInput.value = expr;
                updateSyntaxHighlighting();
                executeExpression();
            });
        });

        // DevExtreme buttons (fallback to native if not available)
        let compileButtonInstance = null;
        let clearButtonInstance = null;
        if (window.DevExpress && DevExpress.ui && DevExpress.ui.dxButton) {
            compileButtonInstance = new DevExpress.ui.dxButton(compileBtn, {
                text: 'Compile & Execute',
                type: 'success',
                stylingMode: 'contained',
                icon: 'chevrondoubleright',
                onClick: () => executeExpression()
            });
            clearButtonInstance = new DevExpress.ui.dxButton(clearBtn, {
                text: 'Clear',
                type: 'danger',
                stylingMode: 'outlined',
                icon: 'trash',
                onClick: () => {
                    expressionInput.value = '';
                    updateSyntaxHighlighting();
                    clearResults();
                }
            });
        } else {
            // Fallback: show plain content and handlers
            compileBtn.innerHTML = '<span class="btn-icon">▶</span> Compile & Execute';
            compileBtn.tabIndex = 0;
            compileBtn.role = 'button';
            compileBtn.addEventListener('click', executeExpression);
            clearBtn.addEventListener('click', () => {
                expressionInput.value = '';
                updateSyntaxHighlighting();
                clearResults();
            });
            clearBtn.innerHTML = '<span class="btn-icon">🗑</span> Clear';
            clearBtn.tabIndex = 0;
            clearBtn.role = 'button';
        }

        // Real-time syntax highlighting
        expressionInput.addEventListener('input', updateSyntaxHighlighting);
        // Keep overlay scroll in sync with textarea
        expressionInput.addEventListener('scroll', () => {
            syntaxOverlay.scrollTop = expressionInput.scrollTop;
            syntaxOverlay.scrollLeft = expressionInput.scrollLeft;
        });

        // Debug tabs
        document.querySelectorAll('.debug-tab').forEach(tab => {
            tab.addEventListener('click', () => {
                const tabName = tab.getAttribute('data-tab');
                switchDebugTab(tabName);
            });
        });

        function updateSyntaxHighlighting() {
            const text = expressionInput.value;
            try {
                const highlighted = self.applySyntaxHighlighting(text);
                syntaxOverlay.innerHTML = highlighted;
            } catch (error) {
                syntaxOverlay.innerHTML = text;
            }
        }

        function executeExpression() {
            const expression = expressionInput.value.trim();
            
            if (!expression) {
                showResult('Please enter an expression', 'error');
                return;
            }

            // Show loading state
            if (compileButtonInstance) {
                compileButtonInstance.option({ text: 'Compiling...', disabled: true });
            } else {
                compileBtn.innerHTML = '<span class="loading"></span> Compiling...';
                compileBtn.disabled = true;
            }

            // Simulate some processing time for better UX
            setTimeout(() => {
                const result = self.execute(expression);
                
                if (result.success) {
                    showResult(result.result, 'success');
                    updateDebugInfo(result);
                } else {
                    showResult(`Error: ${result.error}`, 'error');
                    clearDebugInfo();
                }

                // Reset button
                if (compileButtonInstance) {
                    compileButtonInstance.option({ text: 'Compile & Execute', disabled: false });
                } else {
                    compileBtn.innerHTML = '<span class="btn-icon">▶</span> Compile & Execute';
                    compileBtn.disabled = false;
                }
            }, 100);
        }

        function showResult(result, type) {
            resultOutput.className = `result-output ${type}`;
            resultOutput.innerHTML = `<div>${JSON.stringify(result, null, 2)}</div>`;
        }

        function clearResults() {
            resultOutput.className = 'result-output';
            resultOutput.innerHTML = '<div class="placeholder">Result will appear here...</div>';
            clearDebugInfo();
        }

        function updateDebugInfo(result) {
            if (result.tokens) {
                tokensOutput.textContent = JSON.stringify(result.tokens, null, 2);
            }
            if (result.ast) {
                astOutput.textContent = JSON.stringify(result.ast, null, 2);
            }
            if (result.steps) {
                stepsOutput.textContent = JSON.stringify(result.steps, null, 2);
            }
        }

        function clearDebugInfo() {
            tokensOutput.textContent = '';
            astOutput.textContent = '';
            stepsOutput.textContent = '';
        }

        function switchDebugTab(tabName) {
            // Update tab buttons
            document.querySelectorAll('.debug-tab').forEach(tab => {
                tab.classList.remove('active');
            });
            document.querySelector(`[data-tab="${tabName}"]`).classList.add('active');

            // Update tab content
            document.querySelectorAll('.debug-panel').forEach(panel => {
                panel.classList.remove('active');
            });
            document.getElementById(`${tabName}Debug`).classList.add('active');
        }

        // Initialize DevExtreme Lists for function browser
        if (groupListEl && functionListEl && functionDescriptionEl && window.DevExpress && DevExpress.ui && DevExpress.ui.dxList) {
            const groups = this.functionCatalog.map(g => g.group);
            const groupList = new DevExpress.ui.dxList(groupListEl, {
                items: groups,
                selectionMode: 'single',
                selectedIndex: 0,
                height: 300,
                onSelectionChanged(e) {
                    const groupName = e.addedItems[0];
                    const group = self.functionCatalog.find(g => g.group === groupName);
                    functionList.option('items', group.items.map(i => i.name));
                    functionList.option('selectedIndex', 0);
                    renderFunctionDescription(group.items[0]);
                }
            });

            const initialGroup = this.functionCatalog[0];
            const functionList = new DevExpress.ui.dxList(functionListEl, {
                items: initialGroup.items.map(i => i.name),
                selectionMode: 'single',
                selectedIndex: 0,
                height: 300,
                onSelectionChanged(e) {
                    const selectedName = e.addedItems[0];
                    const groupName = groupList.option('selectedItem');
                    const group = self.functionCatalog.find(g => g.group === groupName);
                    const fnMeta = group.items.find(i => i.name === selectedName);
                    renderFunctionDescription(fnMeta);
                },
                onItemClick(e) {
                    const fnName = e.itemData;
                    const snippet = `[${fnName}()]`;
                    insertAtCursor(expressionInput, snippet);
                    updateSyntaxHighlighting();
                    expressionInput.focus();
                }
            });

            renderFunctionDescription(initialGroup.items[0]);

            function renderFunctionDescription(meta) {
                if (!meta) { functionDescriptionEl.innerHTML = ''; return; }
                functionDescriptionEl.innerHTML = `
                    <div class="fn-title">${meta.name}</div>
                    <div class="fn-signature">${meta.signature}</div>
                    <div class="fn-desc">${meta.description}</div>
                    <div class="fn-example"><span>Example:</span> ${meta.example}</div>
                    <div class="fn-actions"><div id="insertFnBtn" class="dx-button"></div></div>
                `;
                const insertFnBtn = document.getElementById('insertFnBtn');
                if (insertFnBtn && DevExpress.ui && DevExpress.ui.dxButton) {
                    new DevExpress.ui.dxButton(insertFnBtn, {
                        text: 'Insert',
                        icon: 'plus',
                        onClick: () => {
                            const snippet = `[${meta.name}()]`;
                            insertAtCursor(expressionInput, snippet);
                            updateSyntaxHighlighting();
                            expressionInput.focus();
                        }
                    });
                }
            }

            function insertAtCursor(textarea, text) {
                const start = textarea.selectionStart ?? textarea.value.length;
                const end = textarea.selectionEnd ?? textarea.value.length;
                const before = textarea.value.substring(0, start);
                const after = textarea.value.substring(end);
                textarea.value = before + text + after;
                const cursor = start + text.length;
                textarea.selectionStart = textarea.selectionEnd = cursor;
            }
        } else if (groupListEl && functionListEl && functionDescriptionEl) {
            // Fallback simple HTML lists
            const groups = this.functionCatalog.map(g => g.group);
            groupListEl.innerHTML = '<ul class="fallback-list" id="fallbackGroupUl"></ul>';
            functionListEl.innerHTML = '<ul class="fallback-list" id="fallbackFunctionUl"></ul>';
            const groupUl = document.getElementById('fallbackGroupUl');
            const fnUl = document.getElementById('fallbackFunctionUl');

            groups.forEach((grp, idx) => {
                const li = document.createElement('li');
                li.textContent = grp;
                li.className = 'fallback-item' + (idx === 0 ? ' selected' : '');
                li.addEventListener('click', () => selectGroup(grp));
                groupUl.appendChild(li);
            });

            const selectGroup = (grp) => {
                // update selected state
                Array.from(groupUl.children).forEach(li => li.classList.remove('selected'));
                const match = Array.from(groupUl.children).find(li => li.textContent === grp);
                if (match) match.classList.add('selected');

                const group = self.functionCatalog.find(g => g.group === grp);
                fnUl.innerHTML = '';
                group.items.forEach((meta, idx) => {
                    const li = document.createElement('li');
                    li.textContent = meta.name;
                    li.className = 'fallback-item' + (idx === 0 ? ' selected' : '');
                    li.addEventListener('click', () => selectFunction(grp, meta.name));
                    li.addEventListener('dblclick', () => insertSnippet(meta.name));
                    fnUl.appendChild(li);
                });
                renderFunctionDescription(group.items[0]);
            };

            const selectFunction = (grp, name) => {
                Array.from(fnUl.children).forEach(li => li.classList.remove('selected'));
                const match = Array.from(fnUl.children).find(li => li.textContent === name);
                if (match) match.classList.add('selected');
                const group = self.functionCatalog.find(g => g.group === grp);
                const meta = group.items.find(i => i.name === name);
                renderFunctionDescription(meta);
            };

            const insertSnippet = (fnName) => {
                const snippet = `[${fnName}()]`;
                const start = expressionInput.selectionStart ?? expressionInput.value.length;
                const end = expressionInput.selectionEnd ?? expressionInput.value.length;
                const before = expressionInput.value.substring(0, start);
                const after = expressionInput.value.substring(end);
                expressionInput.value = before + snippet + after;
                const cursor = start + snippet.length;
                expressionInput.selectionStart = expressionInput.selectionEnd = cursor;
                updateSyntaxHighlighting();
                expressionInput.focus();
            };

            const renderFunctionDescription = (meta) => {
                if (!meta) { functionDescriptionEl.innerHTML = ''; return; }
                functionDescriptionEl.innerHTML = `
                    <div class="fn-title">${meta.name}</div>
                    <div class="fn-signature">${meta.signature}</div>
                    <div class="fn-desc">${meta.description}</div>
                    <div class="fn-example"><span>Example:</span> ${meta.example}</div>
                    <div class="fn-actions"><button id="fallbackInsertBtn" class="example-btn">Insert</button></div>
                `;
                const btn = document.getElementById('fallbackInsertBtn');
                if (btn) {
                    btn.addEventListener('click', () => insertSnippet(meta.name));
                }
            };

            // initialize with first group
            selectGroup(groups[0]);
        }

        // Variables: define built-in VAR(name) function and UI wiring
        // No VAR function anymore; variables use [@name] syntax

        // Add variable UI (DevExtreme or fallback)
        if (addVarBtnEl && window.DevExpress && DevExpress.ui && DevExpress.ui.dxButton) {
            new DevExpress.ui.dxButton(addVarBtnEl, {
                text: 'Add / Update',
                type: 'default',
                icon: 'add',
                onClick: () => addOrUpdateVar()
            });
        } else if (addVarBtnEl) {
            addVarBtnEl.innerHTML = 'Add / Update';
            addVarBtnEl.classList.add('example-btn');
            addVarBtnEl.addEventListener('click', () => addOrUpdateVar());
        }

        const addOrUpdateVar = () => {
            const name = (varNameInput?.value || '').trim();
            const raw = (varValueInput?.value || '').trim();
            if (!name) return;
            // Try to infer type: number, boolean, or string
            let value;
            if (/^\d+(?:\.\d+)?$/.test(raw)) value = raw.includes('.') ? parseFloat(raw) : parseInt(raw, 10);
            else if (raw === 'true' || raw === 'false') value = (raw === 'true');
            else value = raw;
            self.setVariable(name, value);
            renderVariablesList();
        };

        const renderVariablesList = () => {
            if (!variablesListEl) return;
            const vars = Object.entries(self.variables || {});
            if (!vars.length) { variablesListEl.innerHTML = '<div class="placeholder">No variables yet</div>'; return; }
            variablesListEl.innerHTML = '';
            vars.forEach(([k, v]) => {
                const el = document.createElement('div');
                el.className = 'var-item';
                el.innerHTML = `
                    <div class="var-key">${k}</div>
                    <div class="var-value">${JSON.stringify(v)}</div>
                    <div class="var-actions"><button class="example-btn" data-key="${k}">Insert</button></div>
                `;
                variablesListEl.appendChild(el);
            });
            variablesListEl.querySelectorAll('button.example-btn').forEach(btn => {
                btn.addEventListener('click', () => {
                    const key = btn.getAttribute('data-key');
                    const snippet = `[@${key}]`;
                    const start = expressionInput.selectionStart ?? expressionInput.value.length;
                    const end = expressionInput.selectionEnd ?? expressionInput.value.length;
                    const before = expressionInput.value.substring(0, start);
                    const after = expressionInput.value.substring(end);
                    expressionInput.value = before + snippet + after;
                    const cursor = start + snippet.length;
                    expressionInput.selectionStart = expressionInput.selectionEnd = cursor;
                    updateSyntaxHighlighting();
                    expressionInput.focus();
                });
            });
        };

        // Initial render
        renderVariablesList();
    }
}

// Initialize the application when the DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new ExpressionBuilder();
});
