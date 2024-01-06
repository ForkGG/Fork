module.exports = {
    content: [
        './Pages/**/*.html',
        './Pages/**/*.razor',
        './Shared/**/*.html',
        './Shared/**/*.razor',
        './wwwroot/index.html',
    ],
    safelist: [
        {
            pattern: /(bg|text)-percentage-p(|1|2|3|4|5|6|7|8|9|10)0/,
            variants: ['before']
        },
        {
            pattern: /text-.*/,
            variants: ['hover']
        },
        {
            pattern: /-my-.*/
        },
        {
            pattern: /.*visible/
        },
        {
            pattern: /bg-status-.*/
        }
    ],
    //darkMode: false, // or 'media' or 'class'
    theme: {
        fontSize: {
            'xs': '8pt',
            'sm': '10pt',
            'md': '12pt',
            'base': '12pt',
            'lg': '16px',
            'xl': '20px',
            '2xl': '24px',
            '3xl': '32px'
        },
        colors: {
            transparent: 'transparent',
            current: 'currentColor',
            forkBlue: {
                dark: '#151622',
                DEFAULT: '#1D2030',
                light: '#292C3D',
                hover: '#2C3358',
                highlighted: '#374FC3',
            },
            label: {
                DEFAULT: '#575C82',
                hover: '#E2E3E9',
                selected: '#E2E3E9',
            },
            text: {
                darkest: '#1F2234',
                dark: '#575C82',
                DEFAULT: '#A3A8C1',
                red: '#CE5050',
                orange: '#CEA150',
                green: '#50CE61',
                selected: '#151622',
                selection: '#E2E3E9'
            },
            button: {
                DEFAULT: '#575C82'
            },
            status: {
                inactive: '#575C82',
                orange: '#CEA150',
                green: '#50CE61',
                red: '#CE5050'
            },
            scrollbar: {
                background: '#1D2030',
                thumb: {
                    DEFAULT: '#292C3D',
                    hover: '#313549',
                    active: '#3b4059'
                }
            },
            percentage: {
                p0: '#374FC3',
                p10: '#50CE61',
                p20: '#6DC856',
                p30: '#92C05E',
                p40: '#ABC757',
                p50: '#CEC450',
                p60: '#CEA150',
                p70: '#CE8B50',
                p80: '#B7684A',
                p90: '#CE6150',
                p100: '#CE5050',
            }
        },
        extend: {},
    },
    plugins: [],
}
