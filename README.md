# IPaaS Expression Builder

A powerful frontend application for building and testing expressions in an IPaaS (Integration Platform as a Service) environment, similar to Power Automate's expression builder.

## Features

- **Real-time Syntax Highlighting**: Visual feedback as you type expressions
- **Interactive Expression Builder**: Test expressions with immediate results
- **Comprehensive Function Library**: Built-in functions for common operations
- **Debug Information**: View tokens, AST, and execution steps
- **Example Expressions**: Quick-start examples to get you going
- **Modern UI**: Beautiful, responsive interface with dark/light themes

## Getting Started

1. Open `index.html` in your web browser
2. Start typing expressions in the text area
3. Click "Compile & Execute" to see results
4. Use the example buttons to try different expressions

## Expression Syntax

Expressions follow this format: `[FUNCTION_NAME(arg1, arg2, ...)]`

### Examples

```javascript
[ADD(1,2)]                    // => 3
[CONCAT("Hello", " ", "World")] // => "Hello World"
[IF([EQUALS(1,2)], "True", "False")] // => "False"
[MULTIPLY(5, [ADD(2,3)])]     // => 25
```

## Available Functions

### Mathematical Operations
- `ADD(a, b, ...)` - Adds numbers together
- `SUBTRACT(a, b)` - Subtracts b from a
- `MULTIPLY(a, b, ...)` - Multiplies numbers
- `DIVIDE(a, b)` - Divides a by b

### String Operations
- `CONCAT(str1, str2, ...)` - Concatenates strings
- `LENGTH(str)` - Returns string length
- `UPPER(str)` - Converts to uppercase
- `LOWER(str)` - Converts to lowercase
- `TRIM(str)` - Removes whitespace
- `SUBSTRING(str, start, end)` - Extracts substring
- `CONTAINS(str, substr)` - Checks if string contains substring
- `STARTS_WITH(str, prefix)` - Checks if string starts with prefix
- `ENDS_WITH(str, suffix)` - Checks if string ends with suffix

### Comparison Operations
- `EQUALS(a, b)` - Checks if a equals b
- `NOT_EQUALS(a, b)` - Checks if a does not equal b
- `GREATER(a, b)` - Checks if a > b
- `LESS(a, b)` - Checks if a < b
- `GREATER_EQUAL(a, b)` - Checks if a >= b
- `LESS_EQUAL(a, b)` - Checks if a <= b

### Logical Operations
- `IF(condition, trueVal, falseVal)` - Conditional expression
- `AND(...args)` - Logical AND
- `OR(...args)` - Logical OR
- `NOT(val)` - Logical NOT

## Data Types

- **Numbers**: `1`, `3.14`, `-42`
- **Strings**: `"Hello"`, `'World'` (supports escape sequences)
- **Booleans**: `true`, `false`

## Architecture

The application is built with a clean separation of concerns:

### Lexical Analyzer (Lexer)
- Tokenizes input text into meaningful units
- Handles strings, numbers, functions, brackets, and operators
- Supports escape sequences in strings

### Parser
- Builds an Abstract Syntax Tree (AST) from tokens
- Validates syntax and structure
- Handles nested expressions

### Evaluator
- Executes the AST to produce results
- Implements all built-in functions
- Handles error cases gracefully

### Frontend
- Real-time syntax highlighting
- Interactive debugging interface
- Responsive design for all devices

## File Structure

```
├── index.html          # Main HTML structure
├── styles.css          # CSS styling and responsive design
├── script.js           # JavaScript implementation
└── README.md           # This documentation
```

## Browser Compatibility

- Chrome 60+
- Firefox 55+
- Safari 12+
- Edge 79+

## Contributing

Feel free to extend the function library or improve the UI. The modular architecture makes it easy to add new features.

## License

MIT License - feel free to use this in your own projects!
