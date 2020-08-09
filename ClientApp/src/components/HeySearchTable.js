import React from 'react';
import { useTable } from "react-table";
import moment from 'moment';
import styled from 'styled-components'
import DOMPurify from 'dompurify';

import ExternalLink from './ExternalLink';

const Styles = styled.div`
    padding: 1rem;
    .nowrap {
        white-space: nowrap;
    }
    .image_preview {
        width:100px;
        margin: 0.2em 0;
    }
    table {
        border-spacing: 0;
        thead {
            tr {
                text-align: center;
                color: white;
                background: gray;
            }
        }
        tr {
            :last-child {
                td {
                    border-bottom: 0;
                }
            }
            :nth-child(even) {
                background: aliceblue;
            }
        }
        th,
        td {
            margin: 0;
            padding: 0.5rem;
            :last-child {
                border-right: 0;
            }
        }
    }
`;

/**
 * Column definition for the HeySearch Table.
 * See react-table for the details. 
 */
const columns = [
    {
        Header: "Date/Time",
        accessor: "created",
        Cell: ({value}) => {
            // Using moment library, format date and time 
            const day = moment(value);
            return <span className="nowrap">{day.format('D-MMM-YY')}<br/>{day.format('HH:mm')}</span>;
        }
    },
    {
        Header: "Tweet",
        accessor: "content",
        Cell: (props) => {
            // making sure that the tweet content is sanitize
            // because dangerouslySetInnerHTML is used to embedded a link in the text
            let tweet = DOMPurify.sanitize(props.value);
            tweet = tweet.replace(
                /((http|https):\/\/[a-zA-Z0-9\-.]+\.[a-zA-Z]{2,3}(\/\S*)?)/g,
                '<a href="$1" target="_blank" rel="noopener noreferrer">$1</a>'
            );
    
            return (
                <div>
                    <span dangerouslySetInnerHTML={{__html: tweet}} />
                    <span>
                        &nbsp;
                        <ExternalLink href={`https://twitter.com/${props.row.values.userName}/status/${props.row.values.id}`}>
                            <img width="15" src="/external_link.png" alt="open" />
                        </ExternalLink>
                    </span>
                </div>
            );
        }
    },
    {
        Header: "User",
        accessor: "userName",
        Cell: ({value}) => {
            // make a link to the Twitter user's profile
            return <ExternalLink href={`https://twitter.com/${value}`}>{value}</ExternalLink>
        }
    },
    {
        Header: "Images",
        accessor: "imageUrls",
        Cell: ({value}) => {
            // embedded images if there are image URLs to this tweet.
            return (
                <div>
                {
                    value.map(url => {
                        return (
                            <ExternalLink href={url} key={url}>
                                <img className="image_preview" src={url} alt={url} />
                            </ExternalLink>
                        );
                    })
                }
                </div>
            );
        }
    },
    {
        Header: "Referenced",
        accessor: "originalId",
        Cell: (props) => {
            // if this tweet refers to other tweet (retweet, comment retweet or reply), 
            // make an external link to it. 
            return (props.value ? <ExternalLink href={`https://twitter.com/${props.row.values.originalUserId}/status/${props.value}`}>Referenced Tweet</ExternalLink> : null);
        }
    },
/* below are hidden columns used in other columns */
    {
        Header: "ContentID",
        accessor: "id",
    },
    {
        Header: "UserID",
        accessor: "userId",
    },
    {
        Header: "ReferenceUser",
        accessor: "originalUserId",
    },
];

/**
 * A component to show the Search Result in a table format using react-table.
 * @param {Object} props - react props
 * @param {string} props.data - an array of data for the table
 */
const HeySearchTable = ({ data }) => {
    const { 
        getTableProps, 
        getTableBodyProps, 
        headerGroups, 
        rows, 
        prepareRow,
    } = useTable({ 
        columns, 
        data, 
        initialState : {hiddenColumns: ["id", "userId", "originalUserId"]} 
    });

    const table = (
        <table {...getTableProps()}>
            <thead>
            {
                headerGroups.map(headerGroup => (
                    <tr {...headerGroup.getHeaderGroupProps()}>
                    {   
                        headerGroup.headers.map(column => (
                            <th {...column.getHeaderProps()}>
                            {
                                column.render('Header')
                            }
                            </th>
                        ))
                    }
                    </tr>
                ))
            }
            </thead>
            <tbody {...getTableBodyProps()}>
            {
                rows.map(row => {
                    prepareRow(row);
                    return (
                        <tr {...row.getRowProps()} >
                        {
                            row.cells.map(cell => {
                                return (
                                    <td {...cell.getCellProps()}>
                                    {
                                        cell.render('Cell')
                                    }
                                    </td>
                                )
                            })
                        }
                        </tr>
                    )
                })
            }
            </tbody>
        </table>
    );

    return (data.length ? <Styles>{table}</Styles> : "");
};

export default HeySearchTable;