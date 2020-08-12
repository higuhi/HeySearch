import React from 'react';

const spinnerItemsArray = [
    { transform: 'translate(20 50)', begin: '-0.375s', fill: '#85a2b6' },
    { transform: 'translate(40 50)', begin: '-0.25s',  fill: '#bbcedd' },
    { transform: 'translate(60 50)', begin: '-0.125s', fill: '#dce4eb' },
    { transform: 'translate(80 50)', begin: '-0s',     fill: '#fdfdfd' },
  ];

/**
 * React component to display a simple horizontal loader animation with bubbles in SVG.
 * @param {Object} props - react props
 * @param {Object} props.style - style
 */  
const LoaderAnimation = (props) => {

    return (
        <div style={props.styles}>
            <svg className="loader_animation" xmlns="http://www.w3.org/2000/svg"  width="80px" viewBox="0 0 100 100" preserveAspectRatio="xMidYMid">
            {
                spinnerItemsArray.map((item) => 
                    <g key={item.transform} transform={item.transform}>
                        <circle cx="0" cy="0" r="6"  fill={item.fill}>
                            <animateTransform attributeName="transform" type="scale" begin={item.begin} calcMode="spline" keySplines="0.3 0 0.7 1;0.3 0 0.7 1" values="0;1;0" keyTimes="0;0.5;1" dur="1s" repeatCount="indefinite" />
                        </circle>
                    </g>
                )
            }
            </svg>
        </div>
    );
}

export default LoaderAnimation;