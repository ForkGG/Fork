module.exports = {
    content: [
        './Razor/**/*.html',
        './Razor/**/*.razor',
        './wwwroot/index.html',
    ],
    safelist: [
        {
            pattern: /(bg|text|from|to)-percentage-p(|1|2|3|4|5|6|7|8|9|10)0/,
            variants: ['before']
        }
    ],
    //darkMode: false, // or 'media' or 'class'
    theme: {
        colors: {
            transparent: 'transparent',
            current: 'currentColor',
            new: {
                black: "#0F101A",
                white: "#FFFFFF",
                offwhite: "#747AAC",
                nav: "#131420",
                panel: "#131420",
                light_panel: "#1B1C2D",
                red: "#F76060"
            },
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
